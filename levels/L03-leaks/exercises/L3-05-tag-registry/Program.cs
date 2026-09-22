using TagRegistry;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L3-05-tag-registry: Tag registry",
    Workload: Workload.Run,
    ExpectedChecksum: 1408890,
    MaxMedianMs: 28,
    MaxAllocatedMb: 205,
    MaxRetainedMb: 1,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
