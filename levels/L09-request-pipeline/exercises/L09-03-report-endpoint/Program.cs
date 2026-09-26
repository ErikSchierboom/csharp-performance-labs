using ReportEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-03-report-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 300014235000,
    MaxMedianMs: 62,
    MaxAllocatedMb: 237,
    MaxP99Ms: 5), args);
