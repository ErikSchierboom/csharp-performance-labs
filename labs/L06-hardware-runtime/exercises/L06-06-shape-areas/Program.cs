using ShapeAreas;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-06-shape-areas",
    Workload: Workload.Run,
    ExpectedChecksum: 17622510630,
    MaxMetrics: new() { [Metrics.Time] = 80, [Metrics.Alloc] = 1 }), args);
