using ConfigCache;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-04-config-cache",
    Workload: Workload.Run,
    ExpectedChecksum: 19920,
    MaxMedianMs: 120,
    MaxAllocatedMb: 1,
    MaxCpuMs: 120,
    MaxMetrics: new() { ["factoryCalls"] = 20 }), args);
