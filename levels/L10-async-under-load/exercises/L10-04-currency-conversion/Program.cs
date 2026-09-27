using CurrencyConversion;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-04-currency-conversion",
    Workload: Workload.Run,
    ExpectedChecksum: 80000242400,
    MaxMedianMs: 5,
    MaxAllocatedMb: 2,
    MaxP99Ms: 1,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
