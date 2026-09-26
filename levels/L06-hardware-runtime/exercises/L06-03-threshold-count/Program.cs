using ThresholdCount;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-03-threshold-count",
    Workload: Workload.Run,
    ExpectedChecksum: 5743735661140980,
    MaxMedianMs: 60,
    MaxAllocatedMb: 1), args);
