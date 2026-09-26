using ShapeAreas;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-06-shape-areas",
    Workload: Workload.Run,
    ExpectedChecksum: 17622510630,
    MaxMedianMs: 80,
    MaxAllocatedMb: 1), args);
