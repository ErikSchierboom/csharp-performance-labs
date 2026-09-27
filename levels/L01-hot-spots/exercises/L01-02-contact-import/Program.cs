using ContactImport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-02-contact-import",
    Workload: Workload.Run,
    ExpectedChecksum: 312758,
    MaxMetrics: new() { [Metrics.Time] = 59, [Metrics.Alloc] = 16 }), args);
