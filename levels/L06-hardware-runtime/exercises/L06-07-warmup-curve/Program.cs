using WarmupCurve;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-07-warmup-curve",
    Workload: Workload.Run,
    ExpectedChecksum: 6569391194351585123), args);
