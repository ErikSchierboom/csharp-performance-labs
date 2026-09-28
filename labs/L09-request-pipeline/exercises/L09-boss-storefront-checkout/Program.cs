using StorefrontCheckout;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-boss-storefront-checkout",
    Workload: Workload.Run,
    ExpectedChecksum: 200001550000,
    MaxMetrics: new() { [Metrics.Gen2] = 0, [Metrics.P99] = 15, [Metrics.Time] = 300, [Metrics.Alloc] = 320 }), args);
