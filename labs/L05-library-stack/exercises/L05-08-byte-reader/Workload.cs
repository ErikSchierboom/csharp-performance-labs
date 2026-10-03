namespace ByteReader;

public static class Workload
{
    static readonly string Path = MakeFile();

    public static long Run()
    {
        long sum = 0;
        var bytes = File.ReadAllBytes(Path);
        return bytes.Sum(b => (long)b);
    }

    static string MakeFile()
    {
        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "perflab-bytes-" + Environment.ProcessId + ".bin");
        var data = new byte[400_000];
        new Random(4).NextBytes(data);
        File.WriteAllBytes(path, data);
        AppDomain.CurrentDomain.ProcessExit += (_, _) => File.Delete(path);
        return path;
    }
}
