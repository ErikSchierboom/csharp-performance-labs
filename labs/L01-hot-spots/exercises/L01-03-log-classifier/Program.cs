using LogClassifier;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-03-log-classifier",
    Workload: Workload.Run,
    ExpectedChecksum: 7790206905715,
    MaxMetrics: new() { [Metrics.Time] = 147, [Metrics.Alloc] = 100 }), args);
