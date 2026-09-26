using System.Diagnostics;
using PerfLab.Harness;

namespace WorksOnMyLaptop;

public static class Workload
{
    public static long Run()
    {
        long total = 0;
        Parallel.For(0, 8, new ParallelOptions { MaxDegreeOfParallelism = 8 }, worker =>
        {
            long local = 0;
            for (var i = 0; i < 300_000; i++)
            {
                var request = new byte[2_000]; // short-lived per-request garbage
                request[0] = (byte)i;
                local += request[0];
            }
            Interlocked.Add(ref total, local);
        });

        var info = GC.GetGCMemoryInfo();
        using var p = Process.GetCurrentProcess();
        Lab.Report("committedMB", info.TotalCommittedBytes / 1024f / 1024);
        Lab.Report("workingSetMB", p.WorkingSet64 / 1024f / 1024);
        return total;
    }
}
