using LatencyStats;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-02-latency-stats",
    Workload: Workload.Run,
    ExpectedChecksum: 41179863388,
    MaxMetrics: new() { [Metrics.Time] = 20, [Metrics.Alloc] = 1 }), args);
