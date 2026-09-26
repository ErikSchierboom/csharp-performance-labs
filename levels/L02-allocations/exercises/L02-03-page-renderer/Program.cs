using PageRenderer;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-03-page-renderer",
    Workload: Workload.Run,
    ExpectedChecksum: 2024728565835087958,
    MaxMedianMs: 20,
    MaxAllocatedMb: 1,
    MaxGen2Collections: 2), args);
