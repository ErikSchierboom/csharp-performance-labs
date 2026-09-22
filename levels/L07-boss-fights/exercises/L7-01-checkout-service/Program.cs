using CheckoutService;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L7-01-checkout-service: Checkout service",
    Workload: Workload.Run,
    ExpectedChecksum: -9106395021145220454,
    MaxMedianMs: 15,
    MaxAllocatedMb: 2,
    ScaleTime: false,
    MaxP99Ms: 15,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
