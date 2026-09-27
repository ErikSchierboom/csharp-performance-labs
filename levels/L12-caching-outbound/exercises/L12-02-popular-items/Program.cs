using PopularItems;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-02-popular-items",
    Workload: Workload.Run,
    ExpectedChecksum: 40000121200,
    MaxMedianMs: 70,
    MaxAllocatedMb: 1,
    MaxP99Ms: 70,
    MaxMetrics: new() { [Metrics.Loads] = 5 },
    Reset: Workload.Reset), args);
