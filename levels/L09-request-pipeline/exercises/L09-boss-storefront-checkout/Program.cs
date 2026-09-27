using StorefrontCheckout;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-boss-storefront-checkout",
    Workload: Workload.Run,
    ExpectedChecksum: 200001550000,
    MaxMedianMs: 300,
    MaxAllocatedMb: 320,
    MaxGen2Collections: 0,
    MaxP99Ms: 15), args);
