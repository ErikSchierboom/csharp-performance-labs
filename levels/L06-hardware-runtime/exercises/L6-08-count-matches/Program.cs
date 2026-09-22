using CountMatches;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L6-08-count-matches: Count matches (vectorisation)",
    Workload: Workload.Run,
    ExpectedChecksum: 160759,
    MaxMedianMs: 11,
    MaxAllocatedMb: 1), args);
