using PhantomLeak;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L07-02-phantom-leak",
    Workload: Workload.Run,
    ExpectedChecksum: 2860800,
    MaxMedianMs: 50,
    MaxAllocatedMb: 1,
    MaxRetainedMb: 0,
    MaxMetrics: new() { ["privateMB"] = 0 },
    TimedWarmup: false), args);
