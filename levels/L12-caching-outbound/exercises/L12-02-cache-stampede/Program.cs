using CacheStampede;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-02-cache-stampede: Cache stampede",
    Workload: Workload.Run,
    ExpectedChecksum: 40000121200,
    MaxMedianMs: 221,
    MaxAllocatedMb: 2,
    ScaleTime: false,
    MaxP99Ms: 204,
    MaxMetrics: new() { ["loads"] = 9 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
