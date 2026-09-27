namespace PerfLab.Harness.Data;

/// <summary>
/// Metric names reported via <c>Lab.Report</c> and gated via <c>LabSpec.MaxMetrics</c> by the data-access exercises,
/// so the string used to report a value and the string used to budget it can't drift apart.
/// </summary>
public static class DbMetrics
{
    /// <summary>Total <see cref="CommandCounter"/> commands (see <see cref="CommandCounter.Total"/> or <see cref="CommandCounter.Get"/>).</summary>
    public const string CommandsPerRequest = "commandsPerRequest";

    /// <summary>Rows read per request (see <see cref="RowCounter.Get"/>).</summary>
    public const string RowsPerRequest = "rowsPerRequest";
    
    /// <summary>
    /// Total <see cref="CommandCounter"/> commands (see <see cref="CommandCounter.Total"/> or <see cref="CommandCounter.Get"/>).
    /// </summary>
    public const string TotalCommands = "totalCommands";
}
