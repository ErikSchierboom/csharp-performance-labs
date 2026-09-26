using ShipmentManifest;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-boss-shipment-manifest",
    Workload: Workload.Run,
    ExpectedChecksum: 18981969446,
    MaxMedianMs: 3,
    MaxAllocatedMb: 1), args);
