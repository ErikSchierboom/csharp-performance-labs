using ThresholdCount;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L6-03-threshold-count: Threshold count (branch misprediction)",
    Workload: Workload.Run,
    ExpectedChecksum: 5743735661140980,
    MaxMedianMs: 195,
    MaxAllocatedMb: 3), args);
