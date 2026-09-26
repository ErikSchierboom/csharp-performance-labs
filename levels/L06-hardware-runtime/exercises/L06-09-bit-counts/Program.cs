using BitCounts;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-09-bit-counts",
    Workload: Workload.Run,
    ExpectedChecksum: 128004368,
    MaxMedianMs: 5,
    MaxAllocatedMb: 0), args);
