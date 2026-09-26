using ReferenceData;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-06-reference-data",
    Workload: Workload.Run,
    ExpectedChecksum: 1497000000,
    MaxMedianMs: 20,
    MaxAllocatedMb: 2), args);
