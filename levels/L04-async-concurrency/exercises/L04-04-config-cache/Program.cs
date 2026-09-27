using ConfigCache;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-04-config-cache",
    Workload: Workload.Run,
    ExpectedChecksum: 19920,
    MaxMetrics: new() { [Metrics.FactoryCalls] = 20, [Metrics.Cpu] = 120, [Metrics.Time] = 120, [Metrics.Alloc] = 1 }), args);
