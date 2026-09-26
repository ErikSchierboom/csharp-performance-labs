using HelloEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-01-hello-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 800002476000,
    MaxMedianMs: 25,
    MaxAllocatedMb: 12,
    MaxP99Ms: 1), args);
