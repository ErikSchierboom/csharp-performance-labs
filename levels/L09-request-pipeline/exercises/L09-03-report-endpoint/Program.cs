using ReportEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-03-report-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 300014235000,
    MaxMedianMs: 35,
    MaxAllocatedMb: 100,
    MaxP99Ms: 3), args);
