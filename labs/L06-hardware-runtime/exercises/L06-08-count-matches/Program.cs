using CountMatches;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-08-count-matches",
    Workload: Workload.Run,
    ExpectedChecksum: 1013600,
    MaxMetrics: new() { [Metrics.Time] = 8, [Metrics.Alloc] = 1 }), args);
