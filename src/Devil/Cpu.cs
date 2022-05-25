using System.Diagnostics;
using CommandDotNet;

[Command(Description = "处理器相关操作")]
[Subcommand(RenameAs = "cpu")]
public class Cpu
{
    [Command("consume", Description = "模拟消耗处理器")]
    public void Consume(IConsole console, [Option('u', "used", Description = "百分比值")] int percentage = 20, [Option('t', "time", Description = "持续时间。单位：秒，默认为60秒，如设置小于等于0，则无时间限制")] int time = 60)
    {
        var exit = false;
        var now = DateTime.Now;
        console.CancelKeyPress += (_, __) => exit = true;
        console.WriteLine($"即将消耗{percentage} %CPU，并持续{(time > 0 ? $"{time}秒" : "至主动退出")}...");
        Parallel.For(0, Environment.ProcessorCount, _ =>
        {
            var watch = new Stopwatch();
            watch.Start();
            while (!exit)
            {
                if (time > 0 && (now.AddSeconds(time) < DateTime.Now)) break;
                if (watch.ElapsedMilliseconds > percentage)
                {
                    Thread.Sleep(100 - percentage);
                    watch.Reset();
                    watch.Start();
                }
            }
        });
        console.WriteLine("已完成处理器消耗...");
    }
}