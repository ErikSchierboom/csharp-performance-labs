using OrderPipeline;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-05-order-pipeline",
    Workload: Workload.Run,
    ExpectedChecksum: 3742140,
    MaxMedianMs: 300,
    MaxAllocatedMb: 300,
    MaxMetrics: new() { [Metrics.MaxQueued] = 50, [Metrics.PeakHeapMb] = 12 }), args);
