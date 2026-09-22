using MissingIndex;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L5-03-sku-totals: SKU totals (missing index)",
    Workload: Workload.Run,
    ExpectedChecksum: 201870049,
    MaxMedianMs: 64,
    MaxAllocatedMb: 3), args);
