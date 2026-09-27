using ReportService;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-04-report-service",
    Workload: Workload.Run,
    ExpectedChecksum: 240000730200,
    MaxMedianMs: 10,
    MaxAllocatedMb: 6,
    MaxP99Ms: 1), args);
