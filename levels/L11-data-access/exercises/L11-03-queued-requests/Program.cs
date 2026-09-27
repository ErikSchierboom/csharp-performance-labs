using QueuedRequests;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-03-queued-requests",
    Workload: Workload.Run,
    ExpectedChecksum: 128000385280,
    MaxMedianMs: 500,
    MaxAllocatedMb: 3,
    MaxP99Ms: 50), args);
