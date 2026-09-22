using ShipmentManifest;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "M1-shipment-manifest: Shipment manifest (mixed review after Level 2)",
    Workload: Workload.Run,
    ExpectedChecksum: 18981969446,
    MaxMedianMs: 7,
    MaxAllocatedMb: 3), args);
