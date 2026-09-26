using OrderReport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-boss-order-report",
    Workload: Workload.Run,
    ExpectedChecksum: 1240228,
    MaxMedianMs: 2,
    MaxAllocatedMb: 1), args);
