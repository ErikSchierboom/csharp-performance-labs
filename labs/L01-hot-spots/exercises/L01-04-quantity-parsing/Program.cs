using PerfLab.Harness;
using QuantityParsing;

return Lab.Run(new LabSpec(
    Name: "L01-04-quantity-parsing",
    Workload: Workload.Run,
    ExpectedChecksum: 751376505601,
    MaxMetrics: new() { [Metrics.Time] = 150, [Metrics.Alloc] = 45 }), args);
