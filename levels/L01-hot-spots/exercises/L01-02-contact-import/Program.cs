using ContactImport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-02-contact-import",
    Workload: Workload.Run,
    ExpectedChecksum: 312758,
    MaxMedianMs: 59,
    MaxAllocatedMb: 16), args);
