using ImportEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-05-import-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 240000724800,
    MaxMedianMs: 840,
    MaxAllocatedMb: 450,
    MaxGen2Collections: 2,
    MaxP99Ms: 34), args);
