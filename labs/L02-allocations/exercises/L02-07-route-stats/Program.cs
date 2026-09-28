using RouteStats;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-07-route-stats",
    Workload: Workload.Run,
    ExpectedChecksum: 99457720352575000,
    MaxMetrics: new() { [Metrics.Time] = 20, [Metrics.Alloc] = 2 }), args);
