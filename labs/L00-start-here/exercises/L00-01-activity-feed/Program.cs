using ActivityFeed;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L00-01-activity-feed",
    Workload: Workload.Run,
    ExpectedChecksum: 576201561659499,
    MaxMetrics: new() { [Metrics.Time] = 29, [Metrics.Alloc] = 8 }), args);
