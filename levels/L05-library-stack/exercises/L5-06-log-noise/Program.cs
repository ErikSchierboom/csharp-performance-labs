using LogNoise;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L5-06-log-noise: Log noise (disabled logging isn't free)",
    Workload: Workload.Run,
    ExpectedChecksum: 899997,
    MaxMedianMs: 18,
    MaxAllocatedMb: 1), args);
