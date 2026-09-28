using PriceLookup;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-05-price-lookup",
    Workload: Workload.Run,
    ExpectedChecksum: 3333999775,
    MaxMetrics: new() { [Metrics.Time] = 30, [Metrics.Alloc] = 0.5 }), args);
