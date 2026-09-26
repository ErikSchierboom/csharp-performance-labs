using WarmupCurve;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-07-warmup-curve",
    Workload: Workload.Run,
    ExpectedChecksum: 6569391194351585123,
    MaxMedianMs: 58824,
    MaxAllocatedMb: 1000), args);
