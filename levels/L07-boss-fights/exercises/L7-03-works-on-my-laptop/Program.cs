using ContainerBloat;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L7-03-works-on-my-laptop: Works on my laptop (GC configuration)",
    Workload: Workload.Run,
    ExpectedChecksum: 305971328,
    MaxMedianMs: 740,
    MaxAllocatedMb: 11582,
    MaxMetrics: new() { ["committedMB"] = 102, ["workingSetMB"] = 154 },
    TimedWarmup: false), args);
