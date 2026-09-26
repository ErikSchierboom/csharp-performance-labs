using ImpatientClients;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-05-impatient-clients",
    Workload: Workload.Run,
    ExpectedChecksum: -99999600,
    MaxMedianMs: 2170,
    MaxAllocatedMb: 8,
    ScaleTime: false,
    MaxMetrics: new() { ["stepsAfterAbort"] = 1 },
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
