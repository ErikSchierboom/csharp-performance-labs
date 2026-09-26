using SmallSums;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-08-small-sums",
    Workload: Workload.Run,
    ExpectedChecksum: 552000000,
    MaxMedianMs: 15,
    MaxAllocatedMb: 1), args);
