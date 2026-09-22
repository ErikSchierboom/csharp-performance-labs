using LogClassifier2;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L2-06 Log classifier, part 2",
    Workload: Workload.Run,
    ExpectedChecksum: 19355056251099,
    MaxMedianMs: 30,
    MaxAllocatedMb: 16), args);
