using CustomerProfile;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-02-customer-profile",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241823,
    MaxMedianMs: 75,
    MaxAllocatedMb: 73,
    MaxP99Ms: 9,
    MeasuredRuns: 9), args);
