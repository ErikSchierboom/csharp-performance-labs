using ContactImport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L1-02 Contact import",
    Workload: Workload.Run,
    ExpectedChecksum: 312758,
    MaxMedianMs: 100,
    MaxAllocatedMb: 16), args);
