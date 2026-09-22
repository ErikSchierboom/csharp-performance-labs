using PoolExhaustion;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-04-pool-exhaustion: Connection held across a slow call",
    Workload: Workload.Run,
    ExpectedChecksum: 128000385280,
    MaxMedianMs: 1330,
    MaxAllocatedMb: 6,
    ScaleTime: false,
    MaxP99Ms: 200), args);
