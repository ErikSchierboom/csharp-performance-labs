using ReferenceData;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L4-06-reference-data: Reference data (read-mostly cache)",
    Workload: Workload.Run,
    ExpectedChecksum: 1497000000,
    MaxMedianMs: 34,
    MaxAllocatedMb: 5), args);
