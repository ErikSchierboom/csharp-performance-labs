using ShipmentManifest;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L02-boss-shipment-manifest",
    Workload: Workload.Run,
    ExpectedChecksum: 18981969446,
    MaxMetrics: new() { [Metrics.Time] = 3, [Metrics.Alloc] = 1 }), args);
