using System.Diagnostics;
using System.Runtime.InteropServices;

public class MemoryMetrics
{
    public double Total;
    public double Used;
    public double Free;
    public override string ToString() => $"Total: {Total:N0} MB, Used: {Used:N0} MB, Free: {Free:N0} MB";
}

public class MemoryMetricsClient
{
    public MemoryMetrics GetMetrics()
    {
        if (IsUnix)
        {
            return GetUnixMetrics();
        }

        return GetWindowsMetrics();
    }

    private static bool IsUnix => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    private MemoryMetrics GetWindowsMetrics()
    {
        var output = "";

        var info = new ProcessStartInfo
        {
            FileName = "wmic",
            Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
            RedirectStandardOutput = true
        };

        using (var process = Process.Start(info)!)
        {
            output = process.StandardOutput.ReadToEnd();
        }

        var lines = output.Trim().Split("\n");
        var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
        var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics
        {
            Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0),
            Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0)
        };
        metrics.Used = metrics.Total - metrics.Free;

        return metrics;
    }

    private MemoryMetrics GetUnixMetrics()
    {
        var info = new ProcessStartInfo("free -m")
        {
            FileName = "/bin/bash",
            Arguments = "-c \"free -m > memory\"",
        };

        using var process = Process.Start(info);
        process?.WaitForExit();
        var lines = File.ReadAllLines("memory");
        var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        File.Delete("memory");
        return new MemoryMetrics
        {
            Total = double.Parse(memory[1]),
            Used = double.Parse(memory[2]),
            Free = double.Parse(memory[3])
        };
    }
}