using CatalogService;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-boss-catalog-service",
    Workload: Workload.Run,
    ExpectedChecksum: 300000907500,
    ScaleTime: false,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.Connections] = 49, [Metrics.Retained] = 11, [Metrics.P99] = 128, [Metrics.Time] = 1800, [Metrics.Alloc] = 100 }), args);
