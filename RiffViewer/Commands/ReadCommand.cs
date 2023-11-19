using System.Diagnostics;
using RiffViewer.Lib.Reader;
using RiffViewer.Parameters;

namespace RiffViewer.Commands;

/// <summary>
/// Command that reads and prints a RIFF file
/// </summary>
public class ReadCommand : Command
{
    /// <inheritdoc />
    protected sealed override string Name => "read";

    /// <inheritdoc />
    protected sealed override string Description => "Reads a RIFF file";

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
        SwitchParameter help = new("Help", "Prints help information", new[] { "-h", "--help" }, args, false);
        if (help.Value)
        {
            PrintHelp();
            return;
        }

        Parameter<string> path = new("Input", "Input file to read", new[] { "-i", "--input" }, args, true);
        SwitchParameter noLazy = new("No lazy", "Turns off lazy loading", new[] { "--no-lazy" }, args, false);

        var reader = new RiffReader(path.Value, !noLazy.Value);
        try
        {
            var riffFile = reader.ReadFile();
            Console.WriteLine(riffFile);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("There was an error while reading the file:");
            Console.Error.WriteLine(e.Message);
        }
    }

    /// <inheritdoc />
    public override void PrintHelp()
    {
        Console.WriteLine("Reads a RIFF file and prints its contents");
        Console.WriteLine("Usage: RiffViewer read [options]");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("\n[Name]\t\t[Required]\t[Flags]\t\t[Description]");

        Parameter<string>.PrintHelp("Input", "Input file to read", new[] { "-i", "--input" }, true);
        SwitchParameter.PrintHelp("No lazy", "Turns off lazy loading", new[] { "--no-lazy" }, false);
    }
}