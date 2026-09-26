using PriceEndpoint;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L09-04-price-endpoint",
    Workload: Workload.Run,
    ExpectedChecksum: 300000905631,
    MaxMedianMs: 24,
    MaxAllocatedMb: 11,
    MaxP99Ms: 4), args);
