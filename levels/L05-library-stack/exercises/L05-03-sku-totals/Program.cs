using SkuTotals;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-03-sku-totals",
    Workload: Workload.Run,
    ExpectedChecksum: 201870049,
    MaxMetrics: new() { [Metrics.Time] = 25, [Metrics.Alloc] = 2 }), args);
