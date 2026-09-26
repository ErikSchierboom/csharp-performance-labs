using CaseLookup;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-09-case-lookup",
    Workload: Workload.Run,
    ExpectedChecksum: 32986142,
    MaxMedianMs: 15,
    MaxAllocatedMb: 1), args);
