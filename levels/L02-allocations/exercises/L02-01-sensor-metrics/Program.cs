using SensorMetrics;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-01-sensor-metrics",
    Workload: Workload.Run,
    ExpectedChecksum: 140228330058,
    MaxMedianMs: 25,
    MaxAllocatedMb: 5), args);
