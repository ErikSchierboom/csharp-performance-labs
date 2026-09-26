using JsonResponse;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-05-json-response",
    Workload: Workload.Run,
    ExpectedChecksum: 1212357188583774155,
    MaxMedianMs: 5,
    MaxAllocatedMb: 1), args);
