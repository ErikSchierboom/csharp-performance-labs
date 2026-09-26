using CustomerDashboard;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-05-customer-dashboard",
    Workload: Workload.Run,
    ExpectedChecksum: 28465392998,
    MaxMedianMs: 45,
    MaxAllocatedMb: 5), args);
