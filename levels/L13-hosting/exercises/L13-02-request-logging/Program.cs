using RequestLogging;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L13-02-request-logging",
    Workload: Workload.Run,
    ExpectedChecksum: 300000905445,
    MaxMedianMs: 15,
    MaxAllocatedMb: 10,
    MaxP99Ms: 1), args);
