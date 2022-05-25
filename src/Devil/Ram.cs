using CommandDotNet;

[Command(Description = "内存相关操作")]
[Subcommand(RenameAs = "ram")]
public class Ram
{
    [Command("consume", Description = "模拟消耗剩余内存")]
    public void Consume(IConsole console, [Option('u', "used", Description = "百分比值")] int percentage = 20, [Option('t', "time", Description = "持续时间。单位：秒，默认为60秒，如设置小于等于0，则无时间限制")] int time = 60)
    {
        var manually = new ManualResetEventSlim(false);
        var size = new MemoryMetricsClient().GetMetrics().Free * (percentage / 100.0);
        console.CancelKeyPress += (_, __) => manually.Set();
        console.WriteLine($"即将消耗{size:N0} MB内存，并持续{(time != -1 ? $"{time}秒" : "至主动退出")}...");

        var tmp = new List<byte[]>();
        while (!manually.IsSet && 1024 * size > tmp.Count)
        {
            tmp.Add(new byte[1024]);
        }

        manually.Wait(time > 0 ? TimeSpan.FromSeconds(time) : TimeSpan.FromMilliseconds(-1));
        console.WriteLine("已完成内存消耗...");
    }
}