using MatrixWalk;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-01-matrix-walk",
    Workload: Workload.Run,
    ExpectedChecksum: 8378823002,
    MaxMedianMs: 10,
    MaxAllocatedMb: 0), args);
