using TransformBatch;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-05-transform-batch",
    Workload: Workload.Run,
    ExpectedChecksum: 210000000,
    MaxMedianMs: 40,
    MaxAllocatedMb: 0), args);
