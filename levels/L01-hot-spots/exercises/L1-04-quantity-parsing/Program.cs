using PerfLab.Harness;
using QuantityParsing;

return Lab.Run(new LabSpec(
    Name: "L1-04 Quantity parsing",
    Workload: Workload.Run,
    ExpectedChecksum: 52467993891,
    MaxMedianMs: 100,
    MaxAllocatedMb: 12), args);
