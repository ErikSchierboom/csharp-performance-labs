using OrderReport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L5-boss-order-report: Order report (final boss of Level 5)",
    Workload: Workload.Run,
    ExpectedChecksum: 1240228,
    MaxMedianMs: 6,
    MaxAllocatedMb: 2), args);
