using CaseLookup;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-09-case-lookup",
    Workload: Workload.Run,
    ExpectedChecksum: 32986142,
    MaxMetrics: new() { [Metrics.Time] = 15, [Metrics.Alloc] = 1 }), args);
