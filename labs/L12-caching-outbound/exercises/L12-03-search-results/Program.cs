using SearchResults;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-03-search-results",
    Workload: Workload.Run,
    ExpectedChecksum: 600013800000,
    Reset: Workload.Reset,
    MaxMetrics: new() { [Metrics.Retained] = 5, [Metrics.P99] = 1, [Metrics.Time] = 50, [Metrics.Alloc] = 70 }), args);
