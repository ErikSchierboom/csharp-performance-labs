using ParticleSweep;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-02-particle-sweep",
    Workload: Workload.Run,
    ExpectedChecksum: 24000075999922,
    MaxMedianMs: 5,
    MaxAllocatedMb: 0), args);
