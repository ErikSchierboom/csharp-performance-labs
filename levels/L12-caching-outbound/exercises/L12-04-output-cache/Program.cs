using OutputCaching;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-04-output-cache: Cacheable responses computed every time",
    Workload: Workload.Run,
    ExpectedChecksum: 240000730200,
    MaxMedianMs: 38,
    MaxAllocatedMb: 14,
    ScaleTime: false,
    MaxP99Ms: 6), args);
