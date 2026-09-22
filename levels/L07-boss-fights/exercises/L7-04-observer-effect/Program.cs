using ObserverEffect;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L7-04-observer-effect: Observer effect",
    Workload: Workload.Run,
    ExpectedChecksum: 249199000000,
    MaxMedianMs: 52,
    MaxAllocatedMb: 1), args);
