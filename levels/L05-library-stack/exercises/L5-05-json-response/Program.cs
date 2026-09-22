using JsonResponse;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L5-05-json-response: JSON response",
    Workload: Workload.Run,
    ExpectedChecksum: 4829843394555729230,
    MaxMedianMs: 11,
    MaxAllocatedMb: 2), args);
