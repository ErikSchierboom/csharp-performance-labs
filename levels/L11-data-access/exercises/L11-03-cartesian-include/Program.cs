using CartesianInclude;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-03-cartesian-include: Two collection Includes (row explosion)",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241823,
    MaxMedianMs: 127,
    MaxAllocatedMb: 73,
    MaxP99Ms: 15,
    MeasuredRuns: 9), args);
