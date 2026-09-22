using PhantomLeak;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L7-02-phantom-leak: Phantom leak (native memory)",
    Workload: Workload.Run,
    ExpectedChecksum: 2860800,
    MaxMedianMs: 100,
    ScaleTime: false,   // native-allocator-bound (AllocHGlobal/Free), not CPU throughput; the real gate is MaxRetainedMb/privateMB below
    MaxAllocatedMb: 2,
    MaxRetainedMb: 1,
    MaxMetrics: new() { ["privateMB"] = 1 },
    TimedWarmup: false), args);
