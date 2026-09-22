using LogClassifier;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L1-03 Log classifier",
    Workload: Workload.Run,
    ExpectedChecksum: 7790206905715,
    MaxMedianMs: 250,
    MaxAllocatedMb: 100), args);
