using ImpatientClients;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L10-05-impatient-clients",
    Workload: Workload.Run,
    ExpectedChecksum: 4925015475,
    MaxMedianMs: 2000,
    MaxAllocatedMb: 8,
    ScaleTime: false,
    MaxP99Ms: 1500,
    MaxMetrics: new() { ["stepsAfterAbort"] = 0, ["peakInFlight"] = 25 },
    WarmupRuns: 1,
    MeasuredRuns: 3,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
