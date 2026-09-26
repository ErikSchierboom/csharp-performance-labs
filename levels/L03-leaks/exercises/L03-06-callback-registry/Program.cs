using CallbackRegistry;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-06-callback-registry",
    Workload: Workload.Run,
    ExpectedChecksum: 375876,
    MaxMedianMs: 8,
    MaxAllocatedMb: 58,
    MaxRetainedMb: 1,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
