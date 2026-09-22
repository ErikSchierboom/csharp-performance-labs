using SmallSums;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L2-08-small-sums: Small sums (interface enumeration)",
    Workload: Workload.Run,
    ExpectedChecksum: 552000000,
    MaxMedianMs: 43,
    MaxAllocatedMb: 1), args);
