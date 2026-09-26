using QueuedRequests;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-03-queued-requests",
    Workload: Workload.Run,
    ExpectedChecksum: 128000385280,
    MaxMedianMs: 1330,
    MaxAllocatedMb: 6,
    ScaleTime: false,
    MaxP99Ms: 200), args);
