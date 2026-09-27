using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace PerfLab.Harness;

/// <summary>Describes one exercise: what to run, what "correct" means, and what "fast enough" means.</summary>
/// <param name="Name">Display name for the exercise, printed as the harness banner.</param>
/// <param name="Workload">The exercise's entry point. Runs once per warm-up/measured iteration and returns a checksum.</param>
/// <param name="ExpectedChecksum">The checksum a correct implementation must return. A fast but wrong answer fails with exit code 2.</param>
/// <param name="WarmupRuns">Minimum number of warm-up runs before measurement starts.</param>
/// <param name="MeasuredRuns">Number of measured runs the harness reports on (the median is used for pass/fail).</param>
/// <param name="Reset">
/// Optional callback invoked before every run (warm-up and measured) to clear <i>test scaffolding</i> state, so a
/// leak from one run doesn't pile onto the next and make results depend on how many runs came before it. This is
/// never part of the fix.
/// </param>
/// <param name="TimedWarmup">
/// When <see langword="false"/>, warm-up is exactly <see cref="WarmupRuns"/> runs with no time-based extension,
/// used by leak exercises so warm-up doesn't multiply the leak.
/// </param>
/// <param name="MaxMetrics">
/// Named-metric budgets: the median observed per run must be at or under this value, or that row fails. Every
/// per-run budget lives in this one dictionary, including time and allocation - there's no other way to gate them.
/// Most entries are workload-reported (e.g. <c>sqlCommands</c>, <c>connections</c>) via <see cref="Lab.Report"/>;
/// the harness keeps the maximum <see cref="Lab.Report"/> call per run, then medians across runs. A handful of
/// names are built into the harness itself instead, each measured automatically every run with no workload code
/// needed: <see cref="Metrics.Time"/> (wall-clock time), <see cref="Metrics.Alloc"/> (bytes allocated, MB - never
/// scaled: allocation budgets are absolute and deterministic), <see cref="Metrics.P99"/> (p99 latency, via
/// <see cref="Lab.RecordLatency"/>), <see cref="Metrics.Cpu"/> (CPU time across all threads), <see cref="Metrics.Gen2"/>
/// (full collections per run - Level 2+, LOH-churn problems where total bytes alone doesn't tell the whole story) and
/// <see cref="Metrics.Retained"/> (memory still reachable after a forced full collection - the leak gate for Level 3;
/// a healthy workload retains ~0 between runs). <see cref="Metrics.Time"/>, <see cref="Metrics.P99"/> and
/// <see cref="Metrics.Cpu"/> scale with the machine factor; the rest don't. Omit a key entirely to leave that budget
/// ungated - including <see cref="Metrics.Time"/>/<see cref="Metrics.Alloc"/>: an exercise can be pure exploration
/// (run it, watch whatever metrics you choose to report, nothing to pass or fail) by setting none at all.
/// </param>
/// <param name="ScaleTime">
/// When <see langword="false"/>, <see cref="Metrics.Time"/>, <see cref="Metrics.P99"/> and <see cref="Metrics.Cpu"/>
/// (see <see cref="MaxMetrics"/>) are compared as-is, with no machine-factor scaling. Use this when the workload's
/// time is dominated by a fixed wall-clock wait (<c>Task.Delay</c>, <c>Thread.Sleep</c>, a real socket connect)
/// rather than CPU work: a faster CPU doesn't make those waits shorter, so scaling the budget down for a fast
/// machine would fail a correct fix.
/// </param>
/// <param name="MaxFirstRunMs">
/// Optional budget for the very first run (the first warm-up run: cold JIT, cold caches, an unwarmed thread pool), in
/// reference ms, scaled like <see cref="Metrics.Time"/>. Unlike <see cref="MaxMetrics"/>, this is a single cold
/// measurement, not a median across runs, so it stays its own field. The first run's time is always printed; set
/// this when the exercise is about start-up behaviour that warm-up would otherwise hide. <see cref="double.MaxValue"/>
/// means not gated.
/// </param>
public sealed record LabSpec(
    string Name,
    Func<long> Workload,
    long ExpectedChecksum,
    int WarmupRuns = 2,
    int MeasuredRuns = 5,
    Action? Reset = null,
    bool TimedWarmup = true,
    Dictionary<string, double>? MaxMetrics = null,
    bool ScaleTime = true,
    double MaxFirstRunMs = double.MaxValue);

/// <summary>
/// Modes:
///   (default)     warm up, measure N runs, print a table, PASS/FAIL against the budgets.
///   --profile     run the workload in a loop for --seconds (default 15) so a profiler gets plenty of samples.
///   --calibrate   print this machine's raw spin-loop time, pinned to one core, to (re)set ReferenceSpinMs.
/// Exit codes: 0 = pass, 1 = over budget, 2 = wrong result (you broke behaviour).
/// </summary>
public static class Lab
{
    private static readonly ConcurrentBag<double> Latencies = new();
    // Named "ReportedMetrics" rather than "Metrics" so it doesn't collide with the PerfLab.Harness.Metrics type
    // (the well-known metric-name constants) when this class references both in the same scope.
    private static readonly ConcurrentDictionary<string, double> ReportedMetrics = new();

    /// <summary>Record one operation's latency. Thread-safe. The harness reports the p99 per run.</summary>
    /// <param name="ms">The operation's latency, in milliseconds.</param>
    public static void RecordLatency(double ms) => Latencies.Add(ms);

    /// <summary>Report a named value (e.g. peak queue length). Thread-safe; the harness keeps the maximum per run.</summary>
    /// <param name="name">Metric name, matched against <see cref="LabSpec.MaxMetrics"/>.</param>
    /// <param name="value">The value observed this call; the harness retains the maximum seen per run.</param>
    public static void Report(string name, double value) => ReportedMetrics.AddOrUpdate(name, value, (_, old) => Math.Max(old, value));

    /// <summary>Runs an exercise: dispatches to measured, <c>--profile</c>, or <c>--cold</c> mode based on <paramref name="args"/>. See <see cref="Lab"/> for what each mode does.</summary>
    /// <param name="spec">The exercise's budgets, workload and checksum.</param>
    /// <param name="args">The process's command-line arguments.</param>
    /// <returns>Process exit code: <c>0</c> = pass, <c>1</c> = over budget, <c>2</c> = wrong result (behaviour was broken).</returns>
    public static int Run(LabSpec spec, string[] args)
    {
        Console.WriteLine($"== {spec.Name} ==");
        WarnAboutEnvironment();

        if (args.Contains("--calibrate")) return RunCalibrate();
        if (args.Contains("--cold")) return RunCold(spec, GetInt(args, "--runs", 12));
        return args.Contains("--profile")
            ? Profile(spec, GetInt(args, "--seconds", 15))
            : Measure(spec);
    }

    const int MinWarmupMs = 1000, MaxWarmupRuns = 60;

    private static int Measure(LabSpec spec)
    {
        // Warm up by *time* as well as by count. Tiered JIT promotes hot methods to fully optimised code on a
        // background thread after a ~100 ms quiet period, so two quick runs would leave a correct, fast fix still
        // running its slow tier-0 code in the measured runs. (That is itself a Level 6 topic.)
        var warmStart = Stopwatch.GetTimestamp();
        var firstMs = 0.0;
        var wrongRuns = 0;
        var wrongChecksumSeen = (long?)null;
        for (var i = 0; i < MaxWarmupRuns; i++)
        {
            spec.Reset?.Invoke();
            var runStart = Stopwatch.GetTimestamp();
            var warmResult = spec.Workload();
            if (i == 0) firstMs = Stopwatch.GetElapsedTime(runStart).TotalMilliseconds;
            if (warmResult != spec.ExpectedChecksum) { wrongRuns++; wrongChecksumSeen ??= warmResult; }
            if (i + 1 >= spec.WarmupRuns && (!spec.TimedWarmup || Stopwatch.GetElapsedTime(warmStart).TotalMilliseconds >= MinWarmupMs)) break;
        }

        var factor = spec.ScaleTime ? MachineFactor() : 1.0;
        Console.WriteLine(!spec.ScaleTime
            ? "Time budgets: unscaled (fixed-delay workload, ScaleTime: false)."
            : Math.Abs(factor - 1.0) < 1e-9
                ? "Time budgets: unscaled (PERFLAB_NO_SCALE=1)."
                : ScaledBudgetsBanner(spec, factor));
        if (Math.Abs(spec.MaxFirstRunMs - double.MaxValue) < 1)   // when gated, it gets its own row in the results table instead
            Console.WriteLine($"First run: {firstMs:F1} ms (cold, part of the warm-up, not in the median).");
        Console.WriteLine();

        var metricRuns = new Dictionary<string, List<double>>();
        var proc = Process.GetCurrentProcess();
        Console.WriteLine($"{"run",4} {"ms",10} {"alloc MB",10} {"gen0",5} {"gen1",5} {"gen2",5}");

        for (var i = 1; i <= spec.MeasuredRuns; i++)
        {
            spec.Reset?.Invoke();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            foreach (var (k, v) in ReportedMetrics) { if (!metricRuns.TryGetValue(k, out var list)) metricRuns[k] = list = new(); list.Add(v); }

            var heap0 = GC.GetTotalMemory(false);
            Latencies.Clear(); ReportedMetrics.Clear();
            proc.Refresh(); var cpu0 = proc.TotalProcessorTime;
            int g0 = GC.CollectionCount(0), g1 = GC.CollectionCount(1), g2 = GC.CollectionCount(2);
            var a0 = GC.GetTotalAllocatedBytes(precise: true);
            var t0 = Stopwatch.GetTimestamp();

            var result = spec.Workload();

            var ms = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
            proc.Refresh(); var cpuMs = (proc.TotalProcessorTime - cpu0).TotalMilliseconds;
            var mb = (GC.GetTotalAllocatedBytes(precise: true) - a0) / 1024.0 / 1024.0;

            if (result != spec.ExpectedChecksum) { wrongRuns++; wrongChecksumSeen ??= result; }

            var gen2Count = GC.CollectionCount(2) - g2;
            double? p99 = null;
            if (!Latencies.IsEmpty) { var l = Latencies.OrderBy(x => x).ToArray(); p99 = l[(int)Math.Min(l.Length - 1, Math.Ceiling(l.Length * 0.99) - 1)]; }
            Console.WriteLine($"{i,4} {ms,10:F1} {mb,10:F2} {GC.CollectionCount(0) - g0,5} {GC.CollectionCount(1) - g1,5} {gen2Count,5}");

            GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
            var retainedMb = Math.Max(0, GC.GetTotalMemory(true) - heap0) / 1024.0 / 1024.0;

            Report(Metrics.Time, ms);
            Report(Metrics.Alloc, mb);
            if (p99 is { } p99Value) Report(Metrics.P99, p99Value);
            Report(Metrics.Cpu, cpuMs);
            Report(Metrics.Gen2, gen2Count);
            Report(Metrics.Retained, retainedMb);
        }
        foreach (var (k, v) in ReportedMetrics) { if (!metricRuns.TryGetValue(k, out var list)) metricRuns[k] = list = new(); list.Add(v); }

        var checksumOk = wrongRuns == 0;

        Console.WriteLine();
        Row("metric", "value", "budget", "unit", "result");
        var extraOk = true;
        if (Math.Abs(spec.MaxFirstRunMs - double.MaxValue) > 0)
        {
            double b = spec.MaxFirstRunMs * factor;
            var ok = firstMs <= b; extraOk &= ok;
            Print("first run", firstMs, b, "ms", ok, note: "cold: before any warm-up");
        }
        if (spec.MaxMetrics != null)
            foreach (var (name, max) in spec.MaxMetrics)
            {
                var v = metricRuns.TryGetValue(name, out var list) ? Median(list) : double.NaN;
                var (label, unit, format, scale, note) = BuiltInMetricDisplay.TryGetValue(name, out var meta) ? meta : (name, "", "F0", false, null);
                var b = scale ? max * factor : max;
                var ok = v <= b; extraOk &= ok;
                Print(label, v, b, unit, ok, format, note);
            }
        var pass = checksumOk && extraOk;
        Console.WriteLine(pass ? "\nRESULT: PASS"
            : !checksumOk ? "\nRESULT: wrong result - fix correctness first."
            : "\nRESULT: over budget - keep profiling.");
        return pass ? 0 : !checksumOk ? 2 : 1;
    }

    // Display metadata for the handful of metric names the harness measures itself (see LabSpec.MaxMetrics):
    // the row label (kept as it always printed, e.g. "median kept" rather than "retained"), unit, number format,
    // whether the budget scales with the machine factor, and an optional note. Any other MaxMetrics key is a
    // workload-reported metric and falls back to (its own name, no unit, whole numbers, unscaled, no note).
    private static readonly Dictionary<string, (string Label, string Unit, string Format, bool Scale, string? Note)> BuiltInMetricDisplay = new()
    {
        [Metrics.Time] = ("median time", "ms", "F2", true, null),
        [Metrics.Alloc] = ("median alloc", "MB", "F2", false, null),
        [Metrics.P99] = ("median p99", "ms", "F2", true, null),
        [Metrics.Cpu] = ("median cpu", "ms", "F2", true, "CPU time across all threads"),
        [Metrics.Gen2] = ("median gen2", "", "F0", false, null),
        [Metrics.Retained] = ("median kept", "MB", "F2", false, "still reachable after a full GC"),
    };

    // The "Machine factor Nx vs. reference: time budgets scaled (...)" banner mentions whichever of the
    // machine-scaled budgets (time/p99/cpu) this exercise actually gates - none, one or all three.
    private static string ScaledBudgetsBanner(LabSpec spec, double factor)
    {
        var parts = new List<string>();
        foreach (var (name, label) in new[] { (Metrics.Time, "median"), (Metrics.P99, "p99"), (Metrics.Cpu, "cpu") })
            if (spec.MaxMetrics != null && spec.MaxMetrics.TryGetValue(name, out var max))
                parts.Add($"{label} {max:F1} -> {max * factor:F2} ms");
        return parts.Count == 0
            ? $"Machine factor {factor:F2}x vs. reference."
            : $"Machine factor {factor:F2}x vs. reference: time budgets scaled ({string.Join(", ", parts)}).";
    }

    /// <summary>Prints one result row: the median value against its budget, and PASS/FAIL.</summary>
    private static void Print(string name, double value, double budget, string unit, bool ok, string format = "F2", string? note = null) =>
        Row(name, value.ToString(format), budget.ToString(format), unit, ok ? "PASS" : "FAIL", note);

    // The header and every result row go through here, so the columns always line up.
    private static void Row(string metric, string value, string budget, string unit, string result, string? note = null) =>
        Console.WriteLine($"{metric,-20} {value,10} {budget,10}  {unit,-4} {result,-6}{(note is null ? "" : $"({note})")}".TrimEnd());

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

    private static int Profile(LabSpec spec, int seconds)
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
    // ReferenceSpinMs is this machine's own measured spin time (best of 4, pinned to one core), rounded
    // to 50 ms - the machine every exercise's time-scaled MaxMetrics entries (Metrics.Time/P99/Cpu) are calibrated against. It
    // isn't a "slow box"; it's just the fixed point everything else scales relative to.
    const double ReferenceSpinMs = 50.0;

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

    /// <summary>
    /// --calibrate: prints this machine's own spin-loop time, so you can (re)set ReferenceSpinMs when the
    /// reference machine changes. Pins to one core first - an unpinned process can get scheduled across cores
    /// with different boost clocks, which adds run-to-run noise MachineFactor()'s normal best-of-4 doesn't
    /// fully average out. Unlike MachineFactor(), this is a one-off diagnostic, so it affords a bigger sample.
    /// </summary>
    private static int RunCalibrate()
    {
        var pinned = TryPinToOneCore();
        Console.WriteLine(pinned
            ? "Pinned to one core for a stable reading."
            : "Could not pin to one core (unsupported on this OS, or insufficient permissions) - reading may be noisier.");

        Spin(); // warm up
        var best = double.MaxValue;
        const int samples = 8;
        for (var i = 0; i < samples; i++)
        {
            var t0 = Stopwatch.GetTimestamp();
            Sink = Spin();
            var ms = Stopwatch.GetElapsedTime(t0).TotalMilliseconds;
            best = Math.Min(best, ms);
            Console.WriteLine($"run {i + 1,2}: {ms,8:F2} ms");
        }

        Console.WriteLine();
        Console.WriteLine($"best: {best:F2} ms");
        Console.WriteLine($"Current ReferenceSpinMs is {ReferenceSpinMs:F1} ms (this machine's factor: {best / ReferenceSpinMs:F3}x).");
        Console.WriteLine("To make THIS machine the new reference: set ReferenceSpinMs to the value above (rounded),");
        Console.WriteLine("then rescale every exercise's time-scaled MaxMetrics entries (Metrics.Time/P99/Cpu) by (new / old) to preserve");
        Console.WriteLine("current pass/fail behaviour - don't just change the constant on its own.");
        return 0;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static bool TryPinToOneCore()
    {
#pragma warning disable CA1416 // guarded by try/catch; ProcessorAffinity throws PlatformNotSupportedException on macOS
        try
        {
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)1;
            return true;
        }
        catch
        {
            return false;
        }
#pragma warning restore CA1416
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
