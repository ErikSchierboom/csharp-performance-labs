using AuditMiddleware;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-02-audit-middleware",
    Workload: Workload.Run,
    ExpectedChecksum: 800002415445,
    MaxMetrics: new() { [Metrics.P99] = 1, [Metrics.Time] = 30, [Metrics.Alloc] = 17 }), args);
