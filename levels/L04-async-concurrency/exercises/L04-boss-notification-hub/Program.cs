using NotificationHub;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-boss-notification-hub",
    Workload: Workload.Run,
    ExpectedChecksum: 25909889722,
    MaxMedianMs: 35,
    MaxAllocatedMb: 15,
    MaxRetainedMb: 1,
    MaxMetrics: new() { [Metrics.MaxQueued] = 50 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
