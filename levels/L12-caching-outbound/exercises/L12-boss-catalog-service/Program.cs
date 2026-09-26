using CatalogService;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-boss-catalog-service",
    Workload: Workload.Run,
    ExpectedChecksum: 300000907500,
    MaxMedianMs: 1309,
    MaxAllocatedMb: 153,
    ScaleTime: false,
    MaxRetainedMb: 11,
    MaxP99Ms: 145,
    MaxMetrics: new() { ["connections"] = 49 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
