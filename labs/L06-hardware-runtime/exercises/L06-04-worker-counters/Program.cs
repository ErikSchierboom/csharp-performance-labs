using WorkerCounters;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-04-worker-counters",
    Workload: Workload.Run,
    ExpectedChecksum: 40000000,
    MaxMetrics: new() { [Metrics.Time] = 200, [Metrics.Alloc] = 1 }), args);
