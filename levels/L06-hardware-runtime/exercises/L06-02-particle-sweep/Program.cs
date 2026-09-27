using ParticleSweep;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-02-particle-sweep",
    Workload: Workload.Run,
    ExpectedChecksum: 24000075999922,
    MaxMetrics: new() { [Metrics.Time] = 5, [Metrics.Alloc] = 0 }), args);
