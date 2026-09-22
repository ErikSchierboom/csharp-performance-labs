using PriceLookup;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L2-05 Price lookup",
    Workload: Workload.Run,
    ExpectedChecksum: 3333999775,
    MaxMedianMs: 70,
    MaxAllocatedMb: 4), args);
