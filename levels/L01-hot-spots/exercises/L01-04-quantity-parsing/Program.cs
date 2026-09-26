using PerfLab.Harness;
using QuantityParsing;

return Lab.Run(new LabSpec(
    Name: "L01-04-quantity-parsing",
    Workload: Workload.Run,
    ExpectedChecksum: 751376505601,
    MaxMedianMs: 150,
    MaxAllocatedMb: 45), args);
