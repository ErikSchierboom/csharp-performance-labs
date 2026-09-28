namespace ByteReader;

public static class Workload
{
    static readonly string Path = MakeFile();

    public static long Run()
    {
        long sum = 0;
        // Default buffering: the stream reads 4 KB at a time from the OS and hands out bytes from memory.
        using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        int b;
        while ((b = fs.ReadByte()) != -1) sum += b;
        return sum;
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
