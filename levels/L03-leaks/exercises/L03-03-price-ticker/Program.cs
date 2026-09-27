using PriceTicker;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-03-price-ticker",
    Workload: Workload.Run,
    ExpectedChecksum: 85515,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Retained] = 0, [Metrics.Time] = 5, [Metrics.Alloc] = 20 }), args);
