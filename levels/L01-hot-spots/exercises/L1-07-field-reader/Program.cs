using FieldReader;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L1-07-field-reader: Field reader (reflection in a hot loop)",
    Workload: Workload.Run,
    ExpectedChecksum: 20005599994,
    MaxMedianMs: 14,
    MaxAllocatedMb: 1), args);
