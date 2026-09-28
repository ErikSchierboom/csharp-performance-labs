using AuditLog;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-07-audit-log",
    Workload: Workload.Run,
    ExpectedChecksum: 476000,
    MaxMetrics: new() { [Metrics.Time] = 2, [Metrics.Alloc] = 2 }), args);
