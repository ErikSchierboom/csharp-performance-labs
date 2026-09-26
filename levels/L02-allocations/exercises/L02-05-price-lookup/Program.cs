using PriceLookup;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-05-price-lookup",
    Workload: Workload.Run,
    ExpectedChecksum: 3333999775,
    MaxMedianMs: 30,
    MaxAllocatedMb: 0.5), args);
