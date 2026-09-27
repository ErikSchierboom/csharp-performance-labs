using CheckoutService;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L07-01-checkout-service",
    Workload: Workload.Run,
    ExpectedChecksum: -9106395021145220454,
    MaxFirstRunMs: 30,
    Reset: Workload.Reset,
    MaxMetrics: new() { [Metrics.P99] = 4, [Metrics.Time] = 5, [Metrics.Alloc] = 1 }), args);
