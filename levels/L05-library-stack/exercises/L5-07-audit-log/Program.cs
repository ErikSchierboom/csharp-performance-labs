using AuditLog;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L5-07-audit-log: Audit log (per-call file I/O)",
    Workload: Workload.Run,
    ExpectedChecksum: 476000,
    MaxMedianMs: 7,
    MaxAllocatedMb: 5), args);
