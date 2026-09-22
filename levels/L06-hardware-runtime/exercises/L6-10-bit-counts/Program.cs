using BitCounts;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L6-10-bit-counts: Bit counts (hardware intrinsics)",
    Workload: Workload.Run,
    ExpectedChecksum: 128004368,
    MaxMedianMs: 9,
    MaxAllocatedMb: 1), args);
