using SensorGrid;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-boss-sensor-grid",
    Workload: Workload.Run,
    ExpectedChecksum: 7975907828,
    MaxMetrics: new() { [Metrics.Time] = 24, [Metrics.Alloc] = 1 }), args);
