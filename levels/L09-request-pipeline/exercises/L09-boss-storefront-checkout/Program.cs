using StorefrontCheckout;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-boss-storefront-checkout",
    Workload: Workload.Run,
    ExpectedChecksum: 200001550000,
    MaxMedianMs: 934,
    MaxAllocatedMb: 752,
    ScaleTime: false,
    MaxGen2Collections: 2,
    MaxP99Ms: 44), args);
