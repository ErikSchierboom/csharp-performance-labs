using FieldReader;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-07-field-reader",
    Workload: Workload.Run,
    ExpectedChecksum: 2000055999995,
    MaxMetrics: new() { [Metrics.Time] = 25, [Metrics.Alloc] = 1 }), args);
