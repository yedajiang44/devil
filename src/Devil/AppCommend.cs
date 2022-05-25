
using CommandDotNet;

public class AppCommand
{
    [Subcommand(RenameAs = "cpu")]
    public Cpu? Cpu { get; set; }

    [Subcommand(RenameAs = "ram")]
    public Ram? Ram { get; set; }
}