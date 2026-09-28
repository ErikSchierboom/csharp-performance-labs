using ServiceCalls;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-04-service-calls",
    Workload: Workload.Run,
    ExpectedChecksum: 1200,
    Reset: Workload.Reset,
    MaxMetrics: new() { [Metrics.Connections] = 5, [Metrics.Time] = 20, [Metrics.Alloc] = 4 }), args);
