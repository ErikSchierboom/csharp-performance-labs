using PopularItems;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-02-popular-items",
    Workload: Workload.Run,
    ExpectedChecksum: 40000121200,
    Reset: Workload.Reset,
    MaxMetrics: new() { [Metrics.Loads] = 5, [Metrics.P99] = 70, [Metrics.Time] = 70, [Metrics.Alloc] = 1 }), args);
