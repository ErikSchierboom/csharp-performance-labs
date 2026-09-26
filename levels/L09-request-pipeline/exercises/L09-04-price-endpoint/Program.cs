using PriceEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-04-price-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 300000908523,
    MaxMedianMs: 20,
    MaxAllocatedMb: 6,
    MaxP99Ms: 1), args);
