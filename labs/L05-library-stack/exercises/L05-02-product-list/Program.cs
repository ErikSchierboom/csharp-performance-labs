using ProductList;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L05-02-product-list",
    Workload: Workload.Run,
    ExpectedChecksum: 111555967,
    MaxMetrics: new() { [Metrics.Time] = 10, [Metrics.Alloc] = 2 }), args);
