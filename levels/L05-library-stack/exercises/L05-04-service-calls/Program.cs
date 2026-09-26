using ServiceCalls;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-04-service-calls",
    Workload: Workload.Run,
    ExpectedChecksum: 1200,
    MaxMedianMs: 20,
    MaxAllocatedMb: 4,
    MaxMetrics: new() { ["connections"] = 5 },
    Reset: Workload.Reset), args);
