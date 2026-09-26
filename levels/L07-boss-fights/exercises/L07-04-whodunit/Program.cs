using Whodunit;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L07-04-whodunit",
    Workload: Workload.Run,
    ExpectedChecksum: 249199000000,
    MaxMedianMs: 31,
    MaxAllocatedMb: 1), args);
