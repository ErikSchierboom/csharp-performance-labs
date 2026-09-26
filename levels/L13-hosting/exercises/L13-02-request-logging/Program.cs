using RequestLogging;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L13-02-request-logging",
    Workload: Workload.Run,
    ExpectedChecksum: 300000905445,
    MaxMedianMs: 31,
    MaxAllocatedMb: 25,
    MaxP99Ms: 4), args);
