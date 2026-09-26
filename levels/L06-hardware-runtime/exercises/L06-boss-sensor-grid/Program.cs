using SensorGrid;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L06-boss-sensor-grid",
    Workload: Workload.Run,
    ExpectedChecksum: 7975907828,
    MaxMedianMs: 24,
    MaxAllocatedMb: 1), args);
