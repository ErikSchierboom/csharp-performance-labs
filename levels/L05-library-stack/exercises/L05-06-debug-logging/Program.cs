using DebugLogging;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-06-debug-logging",
    Workload: Workload.Run,
    ExpectedChecksum: 899997,
    MaxMetrics: new() { [Metrics.Time] = 6, [Metrics.Alloc] = 1 }), args);
