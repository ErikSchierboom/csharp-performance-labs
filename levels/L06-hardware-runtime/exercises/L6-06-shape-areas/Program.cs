using ShapeAreas;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L6-06-shape-areas: Shape areas (dispatch)",
    Workload: Workload.Run,
    ExpectedChecksum: 17622510630,
    MaxMedianMs: 247,
    MaxAllocatedMb: 1), args);
