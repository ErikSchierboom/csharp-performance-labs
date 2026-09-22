using TransformBatch;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L6-05-transform-batch: Transform batch (hidden struct copies)",
    Workload: Workload.Run,
    ExpectedChecksum: 210000000,
    MaxMedianMs: 105,
    MaxAllocatedMb: 1), args);
