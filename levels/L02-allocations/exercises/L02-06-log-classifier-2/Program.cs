using LogClassifier2;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-06-log-classifier-2",
    Workload: Workload.Run,
    ExpectedChecksum: 19355056251099,
    MaxMedianMs: 10,
    MaxAllocatedMb: 12), args);
