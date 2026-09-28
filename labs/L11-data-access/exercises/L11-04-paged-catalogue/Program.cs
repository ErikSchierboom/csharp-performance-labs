using PagedCatalogue;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-04-paged-catalogue",
    Workload: Workload.Run,
    ExpectedChecksum: 80000240800,
    Reset: Workload.Reset,
    MaxMetrics: new() { [Metrics.Retained] = 0, [Metrics.P99] = 5, [Metrics.Time] = 60, [Metrics.Alloc] = 70 }), args);
