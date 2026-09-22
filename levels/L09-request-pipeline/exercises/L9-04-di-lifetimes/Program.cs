using DiLifetimes;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L9-04-di-lifetimes: DI lifetimes (a container per request)",
    Workload: Workload.Run,
    ExpectedChecksum: 300000905631,
    MaxMedianMs: 40,
    MaxAllocatedMb: 11,
    MaxP99Ms: 6), args);
