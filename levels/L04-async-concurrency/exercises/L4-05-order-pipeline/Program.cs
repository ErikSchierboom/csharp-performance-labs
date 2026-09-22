using OrderPipeline;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L4-05-order-pipeline: Order pipeline (unbounded queue)",
    Workload: Workload.Run,
    ExpectedChecksum: 373566,
    MaxMedianMs: 106,
    MaxAllocatedMb: 73,
    MaxMetrics: new() { ["maxQueued"] = 76 }), args);
