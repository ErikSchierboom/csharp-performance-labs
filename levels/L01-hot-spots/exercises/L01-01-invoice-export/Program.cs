using InvoiceExport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-01-invoice-export",
    Workload: Workload.Run,
    ExpectedChecksum: 267266806800,
    MaxMedianMs: 24,
    MaxAllocatedMb: 8), args);
