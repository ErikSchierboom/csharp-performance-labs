using Leaderboard;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-06-leaderboard",
    Workload: Workload.Run,
    ExpectedChecksum: 4828399,
    MaxMetrics: new() { [Metrics.Time] = 3, [Metrics.Alloc] = 0.5 }), args);
