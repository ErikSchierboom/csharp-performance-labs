using LatencyStats;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L4-02-latency-stats: Latency stats (lock held too long)",
    Workload: Workload.Run,
    ExpectedChecksum: 41179863388,
    MaxMedianMs: 73,
    MaxAllocatedMb: 1), args);
