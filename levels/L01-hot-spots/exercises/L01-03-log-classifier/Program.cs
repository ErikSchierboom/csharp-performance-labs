using LogClassifier;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-03-log-classifier",
    Workload: Workload.Run,
    ExpectedChecksum: 7790206905715,
    MaxMedianMs: 147,
    MaxAllocatedMb: 100), args);
