using PartnerApiCalls;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-07-partner-api-calls",
    Workload: Workload.Run,
    ExpectedChecksum: 39800,
    MaxMedianMs: 80,
    MaxAllocatedMb: 1,
    ScaleTime: false,
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
