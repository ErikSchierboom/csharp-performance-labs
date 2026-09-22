using HttpClientPerCall;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L5-04-http-client-per-call: HttpClient per call",
    Workload: Workload.Run,
    ExpectedChecksum: 1200,
    MaxMedianMs: 46,
    MaxAllocatedMb: 8,
    ScaleTime: false,
    MaxMetrics: new() { ["connections"] = 7 },
    Reset: Workload.Reset), args);
