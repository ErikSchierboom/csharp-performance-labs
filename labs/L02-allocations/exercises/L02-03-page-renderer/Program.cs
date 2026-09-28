using PageRenderer;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-03-page-renderer",
    Workload: Workload.Run,
    ExpectedChecksum: 2024728565835087958,
    MaxMetrics: new() { [Metrics.Gen2] = 2, [Metrics.Time] = 20, [Metrics.Alloc] = 1 }), args);
