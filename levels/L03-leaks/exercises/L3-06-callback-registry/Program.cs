using CallbackRegistry;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L3-06-callback-registry: Callback registry (closures capture more than you think)",
    Workload: Workload.Run,
    ExpectedChecksum: 375876,
    MaxMedianMs: 8,
    MaxAllocatedMb: 144,
    MaxRetainedMb: 2,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
