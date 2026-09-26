using CurrencyConversion;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-04-currency-conversion",
    Workload: Workload.Run,
    ExpectedChecksum: 80000242400,
    MaxMedianMs: 12,
    MaxAllocatedMb: 3,
    ScaleTime: false,
    MaxP99Ms: 6,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
