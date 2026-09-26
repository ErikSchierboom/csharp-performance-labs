using ImportEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-05-import-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 240000724800,
    MaxMedianMs: 300,
    MaxAllocatedMb: 170,
    MaxGen2Collections: 0,
    MaxP99Ms: 12), args);
