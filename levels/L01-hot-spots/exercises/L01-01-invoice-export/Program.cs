using InvoiceExport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-01-invoice-export",
    Workload: Workload.Run,
    ExpectedChecksum: 267266806800,
    MaxMetrics: new() { [Metrics.Time] = 24, [Metrics.Alloc] = 8 }), args);
