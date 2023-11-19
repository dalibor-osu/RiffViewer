using RiffViewer.Lib.Reader;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Writer;
using RiffViewer.Parameters;

namespace RiffViewer.Commands;

/// <summary>
/// Command that removes a chunk from a RIFF file.
/// </summary>
public class RemoveCommand : Command
{
    /// <inheritdoc />
    protected override string Name => "remove";

    /// <inheritdoc />
    protected override string Description => "Removes a chunk from a RIFF file";

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
        SwitchParameter help = new("Help", "Prints help information", new[] { "-h", "--help" }, args, false);
        if (help.Value)
        {
            PrintHelp();
            return;
        }

        Parameter<string> inputPath = new("Input", "Input file to read", new[] { "-i", "--input" }, args, true);
        Parameter<string> chunkName = new("Chunk", "Name of the chunk to remove", new[] { "-c", "--chunk" }, args,
            true);
        Parameter<string> outputPath = new("Output", "Output file to write", new[] { "-o", "--output" }, args, true);

        var reader = new RiffReader(inputPath.Value, false);
        IRiffFile riffFile;

        try
        {
            riffFile = reader.ReadFile();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("There was an error while reading the file:");
            Console.Error.WriteLine(e.Message);
            return;
        }

        int removedLength = riffFile.RemoveChunk(chunkName.Value);

        if (removedLength < 0)
        {
            Console.Error.WriteLine($"Chunk with name {chunkName.Value} was not found in the file.");
            return;
        }

        var writer = new RiffWriter();
        writer.Write(outputPath.Value, riffFile);
    }

    /// <inheritdoc />
    public override void PrintHelp()
    {
        Console.WriteLine("Removes a chunk from a RIFF file.");
        Console.WriteLine("Usage: RiffViewer remove [options]");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("\n[Name]\t\t[Required]\t[Flags]\t\t[Description]");

        Parameter<string>.PrintHelp("Input", "Input file to read", new[] { "-i", "--input" }, true);
        Parameter<string>.PrintHelp("Output", "Output file to write", new[] { "-o", "--output" }, true);
        Parameter<string>.PrintHelp("Chunk", "Name of the chunk to remove", new[] { "-c", "--chunk" }, true);
    }
}