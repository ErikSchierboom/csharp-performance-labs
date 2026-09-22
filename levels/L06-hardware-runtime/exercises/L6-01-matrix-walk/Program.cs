using MatrixWalk;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L6-01-matrix-walk: Matrix walk (cache locality)",
    Workload: Workload.Run,
    ExpectedChecksum: 8378823002,
    MaxMedianMs: 36,
    MaxAllocatedMb: 1), args);
