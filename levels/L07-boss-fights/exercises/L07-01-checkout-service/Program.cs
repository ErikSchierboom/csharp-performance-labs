using CheckoutService;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L07-01-checkout-service",
    Workload: Workload.Run,
    ExpectedChecksum: -9106395021145220454,
    MaxMedianMs: 5,
    MaxAllocatedMb: 1,
    MaxP99Ms: 4,
    MaxFirstRunMs: 30,
    Reset: Workload.Reset), args);
