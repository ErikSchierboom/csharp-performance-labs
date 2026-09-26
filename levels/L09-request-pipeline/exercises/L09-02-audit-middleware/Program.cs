using AuditMiddleware;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-02-audit-middleware",
    Workload: Workload.Run,
    ExpectedChecksum: 800002415445,
    MaxMedianMs: 30,
    MaxAllocatedMb: 16,
    MaxP99Ms: 1), args);
