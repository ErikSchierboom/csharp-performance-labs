using ConfigCache;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L4-04-config-cache: Config cache (factory runs many times)",
    Workload: Workload.Run,
    ExpectedChecksum: 19920,
    MaxMedianMs: 424,
    MaxAllocatedMb: 2,
    MaxCpuMs: 459,
    MaxMetrics: new() { ["factoryCalls"] = 31 }), args);
