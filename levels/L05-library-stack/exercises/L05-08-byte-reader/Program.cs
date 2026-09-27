using ByteReader;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-08-byte-reader",
    Workload: Workload.Run,
    ExpectedChecksum: 51039148,
    MaxMetrics: new() { [Metrics.Time] = 3, [Metrics.Alloc] = 1 }), args);
