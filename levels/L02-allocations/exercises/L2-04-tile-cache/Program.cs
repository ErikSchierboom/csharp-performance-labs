using TileCache;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L2-04 Tile cache",
    Workload: Workload.Run,
    ExpectedChecksum: -85154446424320,
    MaxMedianMs: 260,
    MaxAllocatedMb: 200), args);
