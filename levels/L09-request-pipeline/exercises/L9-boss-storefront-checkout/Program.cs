using StorefrontCheckout;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L9-boss-storefront-checkout: Storefront checkout (final boss of L9)",
    Workload: Workload.Run,
    ExpectedChecksum: 200001550000,
    MaxMedianMs: 934,
    MaxAllocatedMb: 752,
    ScaleTime: false,
    MaxGen2Collections: 2,
    MaxP99Ms: 44), args);
