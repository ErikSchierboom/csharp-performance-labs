using WideProducts;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L5-02-product-list: Product list (tracking & over-fetching)",
    Workload: Workload.Run,
    ExpectedChecksum: 111555967,
    MaxMedianMs: 25,
    MaxAllocatedMb: 4), args);
