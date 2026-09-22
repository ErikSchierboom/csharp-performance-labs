using BoundedCache;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-03-unbounded-memory-cache: Unbounded MemoryCache",
    Workload: Workload.Run,
    ExpectedChecksum: 600013800000,
    MaxMedianMs: 106,
    MaxAllocatedMb: 168,
    MaxRetainedMb: 9,
    MaxP99Ms: 6,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
