using CountMatches;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-08-count-matches",
    Workload: Workload.Run,
    ExpectedChecksum: 1013600,
    MaxMedianMs: 8,
    MaxAllocatedMb: 1), args);
