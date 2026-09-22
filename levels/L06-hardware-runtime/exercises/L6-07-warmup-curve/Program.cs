using WarmupCurve;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L6-07 Warm-up curve (exploration: no pass/fail gate)",
    Workload: Workload.Run,
    ExpectedChecksum: 6569391194351585123,
    MaxMedianMs: 100000,
    MaxAllocatedMb: 1000), args);
