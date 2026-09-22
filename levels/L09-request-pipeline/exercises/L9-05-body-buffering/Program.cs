using BodyBuffering;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L9-05-body-buffering: Request body (buffered into a string)",
    Workload: Workload.Run,
    ExpectedChecksum: 240000724800,
    MaxMedianMs: 1428,
    MaxAllocatedMb: 450,
    MaxGen2Collections: 2,
    MaxP99Ms: 57), args);
