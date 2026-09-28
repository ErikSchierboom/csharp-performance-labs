using LogClassifier2;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-06-log-classifier-2",
    Workload: Workload.Run,
    ExpectedChecksum: 19355056251099,
    MaxMetrics: new() { [Metrics.Time] = 10, [Metrics.Alloc] = 12 }), args);
