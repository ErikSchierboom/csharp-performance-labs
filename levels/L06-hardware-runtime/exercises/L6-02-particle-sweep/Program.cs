using ParticleSweep;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L6-02-particle-sweep: Particle sweep (pointer chasing)",
    Workload: Workload.Run,
    ExpectedChecksum: 24000075999922,
    MaxMedianMs: 11,
    MaxAllocatedMb: 1), args);
