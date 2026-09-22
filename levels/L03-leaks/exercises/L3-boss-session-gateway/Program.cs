using SessionGateway;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L3-boss-session-gateway: Session gateway (final boss of Level 3)",
    Workload: Workload.Run,
    ExpectedChecksum: 198990,
    MaxMedianMs: 10,
    MaxAllocatedMb: 59,
    MaxRetainedMb: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
