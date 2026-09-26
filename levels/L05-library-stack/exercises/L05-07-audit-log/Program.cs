using AuditLog;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-07-audit-log",
    Workload: Workload.Run,
    ExpectedChecksum: 476000,
    MaxMedianMs: 2,
    MaxAllocatedMb: 2), args);
