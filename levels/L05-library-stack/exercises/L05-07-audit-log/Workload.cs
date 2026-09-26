namespace AuditLog;

public static class Workload
{
    public static long Run()
    {
        var path = Path.Combine(Path.GetTempPath(), "perflab-audit-" + Environment.ProcessId + ".log");
        if (File.Exists(path)) File.Delete(path);
        for (var i = 0; i < 20_000; i++)
            File.AppendAllText(path, $"{i:D6}|user-{i % 50}|action-{i % 9}\n");
        var length = new FileInfo(path).Length;
        File.Delete(path);
        return length;
    }
}
