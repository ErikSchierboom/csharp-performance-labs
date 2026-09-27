using Whodunit;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L07-04-whodunit",
    Workload: Workload.Run,
    ExpectedChecksum: 249199000000,
    MaxMetrics: new() { [Metrics.Time] = 31, [Metrics.Alloc] = 1 }), args);
