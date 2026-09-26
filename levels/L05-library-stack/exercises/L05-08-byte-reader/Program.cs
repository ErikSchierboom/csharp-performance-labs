using ByteReader;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-08-byte-reader",
    Workload: Workload.Run,
    ExpectedChecksum: 51039148,
    MaxMedianMs: 3,
    MaxAllocatedMb: 1), args);
