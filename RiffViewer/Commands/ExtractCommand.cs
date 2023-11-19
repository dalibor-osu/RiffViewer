using RiffViewer.Lib.Reader;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Writer;
using RiffViewer.Parameters;

namespace RiffViewer.Commands;

/// <summary>
/// Command that extracts a chunk from a RIFF file and writes it to a new file
/// </summary>
public class ExtractCommand : Command
{
    /// <inheritdoc />
    protected override string Name => "extract";

    /// <inheritdoc />
    protected override string Description => "Extracts a chunk from a RIFF file";

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
        Parameter<string> chunkName = new("Chunk", "Name of the chunk to extract", new[] { "-c", "--chunk" }, args,
            true);
        string outputPath = $"{inputPath.Value}_{chunkName.Value.Trim()}.rfv";

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

        var chunk = riffFile.FindChunk(chunkName.Value);

        if (chunk == null)
        {
            Console.Error.WriteLine($"Chunk {chunkName.Value} not found in file {inputPath.Value}");
            return;
        }

        var writer = new RiffWriter();
        writer.WriteChunk(outputPath, chunk);

        Console.WriteLine($"Chunk {chunkName.Value} extracted to {outputPath}");
        Console.WriteLine(chunk.ToString());
    }

    /// <inheritdoc />
    public override void PrintHelp()
    {
        Console.WriteLine("Extracts a chunk from a RIFF file and writes it to a new file");
        Console.WriteLine("Usage: RiffViewer extract [options]");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("\n[Name]\t\t[Required]\t[Flags]\t\t[Description]");

        Parameter<string>.PrintHelp("Input", "Input file to read", new[] { "-i", "--input" }, true);
        SwitchParameter.PrintHelp("Chunk", "Name of the chunk to extract", new[] { "-c", "--chunk" }, true);
    }
}