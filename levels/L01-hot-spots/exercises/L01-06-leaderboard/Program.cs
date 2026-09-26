using Leaderboard;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-06-leaderboard",
    Workload: Workload.Run,
    ExpectedChecksum: 4828399,
    MaxMedianMs: 3,
    MaxAllocatedMb: 0.5), args);
