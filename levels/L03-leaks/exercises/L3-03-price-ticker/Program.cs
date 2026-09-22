using PriceTicker;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L3-03-price-ticker: Price ticker",
    Workload: Workload.Run,
    ExpectedChecksum: 85515,
    MaxMedianMs: 8,
    MaxAllocatedMb: 49,
    MaxRetainedMb: 1,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
