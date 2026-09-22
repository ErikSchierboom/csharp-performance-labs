using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace PerfLab.Harness;

/// <summary>Describes one exercise: what to run, what "correct" means, and what "fast enough" means.</summary>
/// <param name="Name">Display name for the exercise, printed as the harness banner.</param>
/// <param name="Workload">The exercise's entry point. Runs once per warm-up/measured iteration and returns a checksum.</param>
/// <param name="ExpectedChecksum">The checksum a correct implementation must return. A fast but wrong answer fails with exit code 2.</param>
/// <param name="MaxMedianMs">Time budget, in reference milliseconds. Scaled to this machine by <see cref="Lab"/>'s machine-factor calibration, unless <see cref="ScaleTime"/> is <see langword="false"/>.</param>
/// <param name="MaxAllocatedMb">Allocation budget, in MB. Never scaled: allocation budgets are absolute and deterministic.</param>
/// <param name="WarmupRuns">Minimum number of warm-up runs before measurement starts.</param>
/// <param name="MeasuredRuns">Number of measured runs the harness reports on (the median is used for pass/fail).</param>
/// <param name="MaxGen2Collections">
/// Optional ceiling on gen2 (full) collections per run. Level 2+ uses it for LOH-churn style problems where total
/// bytes allocated alone doesn't tell the whole story. <see cref="int.MaxValue"/> means this budget isn't gated.
/// </param>
/// <param name="MaxRetainedMb">
/// Optional ceiling on memory still reachable after each run, measured after a forced full collection relative to
/// just before the run. This is the leak gate for Level 3: a healthy workload retains ~0 between runs.
/// <see cref="double.MaxValue"/> means this budget isn't gated.
/// </param>
/// <param name="Reset">
/// Optional callback invoked before every run (warm-up and measured) to clear <i>test scaffolding</i> state, so a
/// leak from one run doesn't pile onto the next and make results depend on how many runs came before it. This is
/// never part of the fix.
/// </param>
/// <param name="TimedWarmup">
/// When <see langword="false"/>, warm-up is exactly <see cref="WarmupRuns"/> runs with no time-based extension,
/// used by leak exercises so warm-up doesn't multiply the leak.
/// </param>
/// <param name="MaxP99Ms">Optional p99 latency budget (Level 4+), in reference ms, scaled like <see cref="MaxMedianMs"/>. Workloads report latencies via <see cref="Lab.RecordLatency"/>.</param>
/// <param name="MaxCpuMs">Optional CPU-time budget (Level 4+), in reference ms, scaled like <see cref="MaxMedianMs"/>.</param>
/// <param name="MaxMetrics">Optional named-metric budgets (e.g. <c>sqlCommands</c>, <c>connections</c>) reported via <see cref="Lab.Report"/>; the harness keeps the maximum observed per run.</param>
/// <param name="ScaleTime">
/// When <see langword="false"/>, <see cref="MaxMedianMs"/>, <see cref="MaxP99Ms"/> and <see cref="MaxCpuMs"/> are
/// compared as-is, with no machine-factor scaling. Use this when the workload's time is dominated by a fixed
/// wall-clock wait (<c>Task.Delay</c>, <c>Thread.Sleep</c>, a real socket connect) rather than CPU work: a faster
/// CPU doesn't make those waits shorter, so scaling the budget down for a fast machine would fail a correct fix.
/// </param>
public sealed record LabSpec(
    string Name,
    Func<long> Workload,
    long ExpectedChecksum,
    double MaxMedianMs,
    double MaxAllocatedMb,
    int WarmupRuns = 2,
    int MeasuredRuns = 5,
    int MaxGen2Collections = int.MaxValue,
    double MaxRetainedMb = double.MaxValue,
    Action? Reset = null,
    bool TimedWarmup = true,
    double MaxP99Ms = double.MaxValue,
    double MaxCpuMs = double.MaxValue,
    Dictionary<string, double>? MaxMetrics = null,
    bool ScaleTime = true);

/// <summary>
/// Two modes:
///   (default)   warm up, measure N runs, print a table, PASS/FAIL against the budgets.
///   --profile   run the workload in a loop for --seconds (default 15) so a profiler gets plenty of samples.
/// Exit codes: 0 = pass, 1 = over budget, 2 = wrong result (you broke behaviour).
/// </summary>
public static class Lab
{
    private static readonly ConcurrentBag<double> Latencies = new();
    private static readonly ConcurrentDictionary<string, double> Metrics = new();

    /// <summary>Record one operation's latency. Thread-safe. The harness reports the p99 per run.</summary>
    /// <param name="ms">The operation's latency, in milliseconds.</param>
    public static void RecordLatency(double ms) => Latencies.Add(ms);

    /// <summary>Report a named value (e.g. peak queue length). Thread-safe; the harness keeps the maximum per run.</summary>
    /// <param name="name">Metric name, matched against <see cref="LabSpec.MaxMetrics"/>.</param>
    /// <param name="value">The value observed this call; the harness retains the maximum seen per run.</param>
    public static void Report(string name, double value) => Metrics.AddOrUpdate(name, value, (_, old) => Math.Max(old, value));

    /// <summary>Runs an exercise: dispatches to measured, <c>--profile</c>, or <c>--cold</c> mode based on <paramref name="args"/>. See <see cref="Lab"/> for what each mode does.</summary>
    /// <param name="spec">The exercise's budgets, workload and checksum.</param>
    /// <param name="args">The process's command-line arguments.</param>
    /// <returns>Process exit code: <c>0</c> = pass, <c>1</c> = over budget, <c>2</c> = wrong result (behaviour was broken).</returns>
    public static int Run(LabSpec spec, string[] args)
    {
        Console.WriteLine($"== {spec.Name} ==");
        WarnAboutEnvironment();

        if (args.Contains("--cold")) return RunCold(spec, GetInt(args, "--runs", 12));
        return args.Contains("--profile")
            ? RunProfile(spec, GetInt(args, "--seconds", 15))
            : RunMeasure(spec);
    }

    const int MinWarmupMs = 1000, MaxWarmupRuns = 60;

    private static int RunMeasure(LabSpec spec)
    {
        // Warm up by *time* as well as by count. Tiered JIT promotes hot methods to fully optimised code on a
        // background thread after a ~100 ms quiet period, so two quick runs would leave a correct, fast fix still
        // running its slow tier-0 code in the measured runs. (That is itself a Level 6 topic.)
        var warmStart = Stopwatch.GetTimestamp();
        for (var i = 0; i < MaxWarmupRuns; i++)
        {
            spec.Reset?.Invoke();
            if (!CheckResult(spec, spec.Workload())) return 2;
            if (i + 1 >= spec.WarmupRuns && (!spec.TimedWarmup || Stopwatch.GetElapsedTime(warmStart).TotalMilliseconds >= MinWarmupMs)) break;
        }

        var factor = spec.ScaleTime ? MachineFactor() : 1.0;
        var timeBudget = spec.MaxMedianMs * factor;
        Console.WriteLine(!spec.ScaleTime
            ? "Time budgets: unscaled (fixed-delay workload, ScaleTime: false)."
            : Math.Abs(factor - 1.0) < 1e-9
                ? "Time budgets: unscaled (PERFLAB_NO_SCALE=1)."
                : $"Machine factor {factor:F2}x vs. reference (budget {spec.MaxMedianMs:F0} ms -> {timeBudget:F0} ms on this machine).");
        Console.WriteLine();

        var times = new List<double>();
        var allocs = new List<double>();
        var gen2s = new List<double>();
        var retained = new List<double>();
        var p99s = new List<double>();
        var cpus = new List<double>();
        var metricRuns = new Dictionary<string, List<double>>();
        var proc = Process.GetCurrentProcess();
        Console.WriteLine($"{"run",4} {"ms",10} {"alloc MB",10} {"gen0",5} {"gen1",5} {"gen2",5}");

        for (var i = 1; i <= spec.MeasuredRuns; i++)
        {
            spec.Reset?.Invoke();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var heap0 = GC.GetTotalMemory(false);
            Latencies.Clear(); Metrics.Clear();
            proc.Refresh(); var cpu0 = proc.TotalProcessorTime;
            int g0 = GC.CollectionCount(0), g1 = GC.CollectionCount(1), g2 = GC.CollectionCount(2);
            var a0 = GC.GetTotalAllocatedBytes(precise: true);
            var t0 = Stopwatch.GetTimestamp();

            var result = spec.Workload();

            var ms = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
            proc.Refresh(); var cpuMs = (proc.TotalProcessorTime - cpu0).TotalMilliseconds;
            var mb = (GC.GetTotalAllocatedBytes(precise: true) - a0) / 1024.0 / 1024.0;

            if (!CheckResult(spec, result)) return 2;

            times.Add(ms);
            allocs.Add(mb);
            cpus.Add(cpuMs);
            if (!Latencies.IsEmpty) { var l = Latencies.OrderBy(x => x).ToArray(); p99s.Add(l[(int)Math.Min(l.Length - 1, Math.Ceiling(l.Length * 0.99) - 1)]); }
            foreach (var (k, v) in Metrics) { if (!metricRuns.TryGetValue(k, out var list)) metricRuns[k] = list = new(); list.Add(v); }
            gen2s.Add(GC.CollectionCount(2) - g2);
            Console.WriteLine($"{i,4} {ms,10:F1} {mb,10:F2} {GC.CollectionCount(0) - g0,5} {GC.CollectionCount(1) - g1,5} {GC.CollectionCount(2) - g2,5}");

            // What is still reachable now? (After the row is printed so these collections don't pollute the GC columns.)
            GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
            retained.Add(Math.Max(0, GC.GetTotalMemory(true) - heap0) / 1024.0 / 1024.0);
        }

        double medMs = Median(times), medMb = Median(allocs);
        var timeOk = medMs <= timeBudget;
        var allocOk = medMb <= spec.MaxAllocatedMb;
        var medGen2 = Median(gen2s);
        var gen2Gated = spec.MaxGen2Collections != int.MaxValue;
        var gen2Ok = !gen2Gated || medGen2 <= spec.MaxGen2Collections;
        var medRet = Median(retained);
        var retGated = Math.Abs(spec.MaxRetainedMb - double.MaxValue) > 0;
        var retOk = !retGated || medRet <= spec.MaxRetainedMb;

        Console.WriteLine();
        Console.WriteLine($"median time:\t{medMs,9:F1} ms\tbudget {timeBudget,8:F1} ms\t{(timeOk ? "PASS" : "FAIL")}");
        Console.WriteLine($"median alloc:\t{medMb,9:F2} MB\tbudget {spec.MaxAllocatedMb,8:F2} MB\t{(allocOk ? "PASS" : "FAIL")}");
        if (gen2Gated)
            Console.WriteLine($"median gen2:\t{medGen2,9:F0}\tbudget {spec.MaxGen2Collections,8}\t{(gen2Ok ? "PASS" : "FAIL")}");
        if (retGated)
            Console.WriteLine($"median kept:\t{medRet,9:F2} MB\tbudget {spec.MaxRetainedMb,8:F2} MB\t{(retOk ? "PASS" : "FAIL")}   (still reachable after a full GC)");
        var extraOk = true;
        if (Math.Abs(spec.MaxP99Ms - double.MaxValue) > 0)
        {
            double p99 = p99s.Count > 0 ? Median(p99s) : double.NaN, b = spec.MaxP99Ms * factor;
            var ok = p99 <= b; extraOk &= ok;
            Console.WriteLine($"median p99  : {p99,9:F1} ms   budget {b,8:F1} ms   {(ok ? "PASS" : "FAIL")}");
        }
        if (Math.Abs(spec.MaxCpuMs - double.MaxValue) > 0)
        {
            double cpu = Median(cpus), b = spec.MaxCpuMs * factor;
            var ok = cpu <= b; extraOk &= ok;
            Console.WriteLine($"median cpu  : {cpu,9:F1} ms   budget {b,8:F1} ms   {(ok ? "PASS" : "FAIL")}   (CPU time across all threads)");
        }
        if (spec.MaxMetrics != null)
            foreach (var (name, max) in spec.MaxMetrics)
            {
                var v = metricRuns.TryGetValue(name, out var list) ? Median(list) : double.NaN;
                var ok = v <= max; extraOk &= ok;
                Console.WriteLine($"metric {name} : {v,9:F0}      budget {max,8:F0}      {(ok ? "PASS" : "FAIL")}");
            }
        var pass = timeOk && allocOk && gen2Ok && retOk && extraOk;
        Console.WriteLine(pass ? "\nRESULT: PASS" : "\nRESULT: over budget - keep profiling.");
        return pass ? 0 : 1;
    }

    /// <summary>
    /// --cold [--runs N]: no warm-up, no budgets. Prints every run so you can watch tiered JIT, OSR and dynamic PGO
    /// take effect (run 1 is usually the slowest). Use with DOTNET_TieredPGO=0, DOTNET_TieredCompilation=0, etc.
    /// </summary>
    private static int RunCold(LabSpec spec, int runs)
    {
        Console.WriteLine($"Cold mode: {runs} runs, no warm-up, no budgets.  (env: {string.Join(' ', Environment.GetEnvironmentVariables().Keys.Cast<string>().Where(k => k.StartsWith("DOTNET_")).Select(k => k + "=" + Environment.GetEnvironmentVariable(k)))})");
        Console.WriteLine($"{"run",4} {"ms",10} {"alloc MB",10}");
        for (var i = 1; i <= runs; i++)
        {
            spec.Reset?.Invoke();
            var a0 = GC.GetTotalAllocatedBytes(precise: true);
            var t0 = Stopwatch.GetTimestamp();
            var result = spec.Workload();
            var ms = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
            var mb = (GC.GetTotalAllocatedBytes(precise: true) - a0) / 1024.0 / 1024.0;
            if (!CheckResult(spec, result)) return 2;
            Console.WriteLine($"{i,4} {ms,10:F1} {mb,10:F2}");
        }
        return 0;
    }

    private static int RunProfile(LabSpec spec, int seconds)
    {
        Console.WriteLine($"Profile mode: looping for ~{seconds}s. Attach/start your profiler now if you haven't.");
        if (!CheckResult(spec, spec.Workload())) return 2;   // one warm-up so JIT noise is out of the way

        var end = Stopwatch.GetTimestamp() + seconds * Stopwatch.Frequency;
        var runs = 0;
        while (Stopwatch.GetTimestamp() < end)
        {
            if (!CheckResult(spec, spec.Workload())) return 2;
            runs++;
        }
        Console.WriteLine($"Done: {runs} iterations.");
        return 0;
    }

    // ---- machine-speed calibration -------------------------------------------------------------
    // Time budgets in the exercises are written in "reference milliseconds". We time a fixed CPU loop
    // and scale budgets by (this machine / reference machine), so a fast laptop can't pass by accident
    // and a slow CI box doesn't fail spuriously. Allocation budgets are deterministic and never scaled.
    // Set PERFLAB_NO_SCALE=1 to disable globally, or LabSpec.ScaleTime=false per exercise for workloads
    // whose time is a fixed wall-clock wait (Task.Delay/Thread.Sleep/a real connect), not CPU work: a
    // faster CPU doesn't shrink those, so this factor would otherwise scale their budget down wrongly.
    const double ReferenceSpinMs = 85.0;

    private static double MachineFactor()
    {
        if (Environment.GetEnvironmentVariable("PERFLAB_NO_SCALE") == "1") return 1.0;
        Spin(); // warm up
        var best = double.MaxValue;
        for (var i = 0; i < 4; i++)
        {
            var t0 = Stopwatch.GetTimestamp();
            Sink = Spin();
            best = Math.Min(best, Stopwatch.GetElapsedTime(t0).TotalMilliseconds);
        }
        return best / ReferenceSpinMs;
    }

    private static long Sink;

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveOptimization)]
    private static long Spin()
    {
        var x = 88172645463325252UL;
        long acc = 0;
        for (var i = 0; i < 40_000_000; i++)
        {
            x ^= x << 13; x ^= x >> 7; x ^= x << 17;
            acc += (long)(x & 0xFF);
        }
        return acc;
    }

    private static bool CheckResult(LabSpec spec, long actual)
    {
        if (actual == spec.ExpectedChecksum) return true;
        Console.WriteLine($"WRONG RESULT: checksum {actual}, expected {spec.ExpectedChecksum}. A 'fast' answer that is wrong doesn't count.");
        return false;
    }

    private static void WarnAboutEnvironment()
    {
        var entry = Assembly.GetEntryAssembly();
        var dbg = entry?.GetCustomAttribute<DebuggableAttribute>();
        
        if (dbg is { IsJITOptimizerDisabled: true })
            Console.WriteLine("!! Debug build detected (JIT optimizer disabled). Numbers are meaningless - use -c Release.");
        
        if (Debugger.IsAttached)
            Console.WriteLine("!! Debugger attached. Use 'Profile', not 'Debug' (a debugger changes exception cost, JIT, and timing).");
        
        Console.WriteLine($"Runtime: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}, " +
                          $"GC: {(System.Runtime.GCSettings.IsServerGC ? "Server" : "Workstation")}, cores: {Environment.ProcessorCount}\n");
    }

    private static double Median(List<double> xs)
    {
        var s = xs.OrderBy(x => x).ToList();
        return s.Count % 2 == 1 ? s[s.Count / 2] : (s[s.Count / 2 - 1] + s[s.Count / 2]) / 2;
    }

    private static int GetInt(string[] args, string name, int fallback)
    {
        int i = Array.IndexOf(args, name);
        return i >= 0 && i + 1 < args.Length && int.TryParse(args[i + 1], out var v) ? v : fallback;
    }
}
