using MetricsBoxing;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L2-01 Metrics boxing",
    Workload: Workload.Run,
    ExpectedChecksum: 140228330058,
    MaxMedianMs: 60,
    MaxAllocatedMb: 12), args);
