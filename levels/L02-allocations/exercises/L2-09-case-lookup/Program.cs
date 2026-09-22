using CaseLookup;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L2-09-case-lookup: Case lookup (ToLower as a key)",
    Workload: Workload.Run,
    ExpectedChecksum: 32986142,
    MaxMedianMs: 43,
    MaxAllocatedMb: 1), args);
