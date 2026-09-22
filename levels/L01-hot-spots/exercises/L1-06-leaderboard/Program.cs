using Leaderboard;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L1-06-leaderboard: Leaderboard (sorting inside a loop)",
    Workload: Workload.Run,
    ExpectedChecksum: 4828399,
    MaxMedianMs: 5,
    MaxAllocatedMb: 2), args);
