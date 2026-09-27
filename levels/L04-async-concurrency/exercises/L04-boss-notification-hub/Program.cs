using NotificationHub;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-boss-notification-hub",
    Workload: Workload.Run,
    ExpectedChecksum: 25909889722,
    Reset: Workload.Reset,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.MaxQueued] = 50, [Metrics.Retained] = 1, [Metrics.Time] = 35, [Metrics.Alloc] = 15 }), args);
