using SensorMetrics;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-01-sensor-metrics",
    Workload: Workload.Run,
    ExpectedChecksum: 140228330058,
    MaxMetrics: new() { [Metrics.Time] = 25, [Metrics.Alloc] = 5 }), args);
