using LatencyStats;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-02-latency-stats",
    Workload: Workload.Run,
    ExpectedChecksum: 41179863388,
    MaxMedianMs: 20,
    MaxAllocatedMb: 1), args);
