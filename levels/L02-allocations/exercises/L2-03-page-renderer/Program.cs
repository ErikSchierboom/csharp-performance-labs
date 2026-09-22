using PageRenderer;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L2-03 Page renderer",
    Workload: Workload.Run,
    ExpectedChecksum: 2024728565835087958,
    MaxMedianMs: 80,
    MaxAllocatedMb: 8,
    MaxGen2Collections: 2), args);
