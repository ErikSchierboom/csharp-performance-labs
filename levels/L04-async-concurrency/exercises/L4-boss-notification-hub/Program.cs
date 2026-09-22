using NotificationHub;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "M2-notification-hub: Notification hub (mixed review after Level 4)",
    Workload: Workload.Run,
    ExpectedChecksum: 25909889722,
    MaxMedianMs: 113,
    MaxAllocatedMb: 37,
    MaxRetainedMb: 1,
    MaxMetrics: new() { ["maxQueued"] = 76 },
    Reset: Workload.Reset,
    TimedWarmup: false), args);
