using QueuedRequests;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-03-queued-requests",
    Workload: Workload.Run,
    ExpectedChecksum: 128000385280,
    MaxMetrics: new() { [Metrics.P99] = 50, [Metrics.Time] = 500, [Metrics.Alloc] = 3 }), args);
