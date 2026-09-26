using ProductList;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-02-product-list",
    Workload: Workload.Run,
    ExpectedChecksum: 111555967,
    MaxMedianMs: 10,
    MaxAllocatedMb: 2), args);
