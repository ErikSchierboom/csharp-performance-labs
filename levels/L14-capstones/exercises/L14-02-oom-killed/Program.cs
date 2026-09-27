using OomKilled;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L14-02-oom-killed",
    Workload: Workload.Run,
    ExpectedChecksum: 240000727200,
    MaxMedianMs: 15,
    MaxAllocatedMb: 10,
    MaxGen2Collections: 0,
    MaxRetainedMb: 1,
    MaxP99Ms: 1,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
