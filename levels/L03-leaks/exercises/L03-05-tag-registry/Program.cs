using TagRegistry;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-05-tag-registry",
    Workload: Workload.Run,
    ExpectedChecksum: 1408890,
    MaxMedianMs: 10,
    MaxAllocatedMb: 90,
    MaxRetainedMb: 0,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
