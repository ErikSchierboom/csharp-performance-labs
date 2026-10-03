namespace AuditLog;

public static class Workload
{
    public static long Run()
    {
        var path = Path.Combine(Path.GetTempPath(), "perflab-audit-" + Environment.ProcessId + ".log");

        using var fileStream = File.OpenWrite(path);
        using var fileStreamWriter = new StreamWriter(fileStream);

        for (var i = 0; i < 20_000; i++)
            fileStreamWriter.Write($"{i:D6}|user-{i % 50}|action-{i % 9}\n");

        fileStreamWriter.Flush();
        var length = fileStream.Length;
        fileStreamWriter.Close();

        return length;
    }
}
