using ReferenceData;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-06-reference-data",
    Workload: Workload.Run,
    ExpectedChecksum: 1497000000,
    MaxMetrics: new() { [Metrics.Time] = 20, [Metrics.Alloc] = 2 }), args);
