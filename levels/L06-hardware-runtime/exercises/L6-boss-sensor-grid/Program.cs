using SensorGrid;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "M3-sensor-grid: Sensor grid (mixed review after Level 6)",
    Workload: Workload.Run,
    ExpectedChecksum: 7975907828,
    MaxMedianMs: 40,
    MaxAllocatedMb: 1), args);
