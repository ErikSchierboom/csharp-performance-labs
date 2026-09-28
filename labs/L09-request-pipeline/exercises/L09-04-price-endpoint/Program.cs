using PriceEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-04-price-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 300000908523,
    MaxMetrics: new() { [Metrics.P99] = 1, [Metrics.Time] = 20, [Metrics.Alloc] = 6 }), args);
