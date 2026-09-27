using CatalogService;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-boss-catalog-service",
    Workload: Workload.Run,
    ExpectedChecksum: 300000907500,
    MaxMedianMs: 1800,
    MaxAllocatedMb: 100,
    ScaleTime: false,
    MaxRetainedMb: 11,
    MaxP99Ms: 128,
    MaxMetrics: new() { [Metrics.Connections] = 49 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
