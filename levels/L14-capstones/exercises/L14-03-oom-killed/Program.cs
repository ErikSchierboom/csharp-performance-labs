using OomKilled;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L14-03-oom-killed: OOM-killed at 3 a.m. (capstone)",
    Workload: Workload.Run,
    ExpectedChecksum: 240000727200,
    MaxMedianMs: 42,
    MaxAllocatedMb: 10,
    MaxGen2Collections: 2,
    MaxRetainedMb: 2,
    MaxP99Ms: 6,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
