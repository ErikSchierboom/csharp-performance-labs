using TileCache;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-04-tile-cache",
    Workload: Workload.Run,
    ExpectedChecksum: -85154446424320,
    MaxMedianMs: 100,
    MaxAllocatedMb: 200), args);
