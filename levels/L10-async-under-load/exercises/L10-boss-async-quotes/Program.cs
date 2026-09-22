using AsyncQuotes;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-boss-async-quotes: Async quotes (final boss of Level 10)",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241892,
    MaxMedianMs: 2164,
    MaxAllocatedMb: 9,
    ScaleTime: false,
    MaxP99Ms: 621,
    MaxMetrics: new() { ["peakInflight"] = 61 },
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
