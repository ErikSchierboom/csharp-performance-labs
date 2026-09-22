using ByteReader;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L5-08-byte-reader: Byte reader (unbuffered file reads)",
    Workload: Workload.Run,
    ExpectedChecksum: 51039148,
    MaxMedianMs: 5,
    MaxAllocatedMb: 1), args);
