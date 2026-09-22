using CustomerDashboard;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L1-05 Customer dashboard",
    Workload: Workload.Run,
    ExpectedChecksum: 28465392998,
    MaxMedianMs: 150,
    MaxAllocatedMb: 8), args);
