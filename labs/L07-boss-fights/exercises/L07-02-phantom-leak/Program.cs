using PhantomLeak;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L07-02-phantom-leak",
    Workload: Workload.Run,
    ExpectedChecksum: 2860800,
    TimedWarmup: false,
    MaxMetrics: new() { [Metrics.PrivateMb] = 0, [Metrics.Retained] = 0, [Metrics.Time] = 50, [Metrics.Alloc] = 1 }), args);
