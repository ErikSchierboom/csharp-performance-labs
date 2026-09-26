using SessionGateway;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-boss-session-gateway",
    Workload: Workload.Run,
    ExpectedChecksum: 198990,
    MaxMedianMs: 4,
    MaxAllocatedMb: 24,
    MaxRetainedMb: 1,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
