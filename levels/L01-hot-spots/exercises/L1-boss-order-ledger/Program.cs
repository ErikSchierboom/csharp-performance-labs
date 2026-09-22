using OrderLedger;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L1-boss-order-ledger: Order ledger (final boss of Level 1)",
    Workload: Workload.Run,
    ExpectedChecksum: 4828905301,
    MaxMedianMs: 8,
    MaxAllocatedMb: 9), args);
