using OomKilled;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L14-02-oom-killed",
    Workload: Workload.Run,
    ExpectedChecksum: 240000727200,
    MaxMedianMs: 25,
    MaxAllocatedMb: 10,
    MaxGen2Collections: 2,
    MaxRetainedMb: 2,
    MaxP99Ms: 4,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
