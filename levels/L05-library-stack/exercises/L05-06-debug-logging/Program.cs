using DebugLogging;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-06-debug-logging",
    Workload: Workload.Run,
    ExpectedChecksum: 899997,
    MaxMedianMs: 6,
    MaxAllocatedMb: 1), args);
