using PriceTicker;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-03-price-ticker",
    Workload: Workload.Run,
    ExpectedChecksum: 85515,
    MaxMedianMs: 5,
    MaxAllocatedMb: 20,
    MaxRetainedMb: 0,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
