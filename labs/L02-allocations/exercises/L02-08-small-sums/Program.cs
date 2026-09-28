using SmallSums;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-08-small-sums",
    Workload: Workload.Run,
    ExpectedChecksum: 552000000,
    MaxMetrics: new() { [Metrics.Time] = 15, [Metrics.Alloc] = 1 }), args);
