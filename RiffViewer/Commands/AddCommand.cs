using RiffViewer.Lib.Reader;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.Lib.Writer;
using RiffViewer.Parameters;

namespace RiffViewer.Commands;

/// <summary>
/// Command to add a chunk to a RIFF file
/// </summary>
public class AddCommand : Command
{
    /// <inheritdoc />
    protected override string Name => "add";
    
    /// <inheritdoc />
    protected override string Description => "Adds a chunk to a RIFF file";
    
    /// <inheritdoc />
    public override void Execute(string[] args)
    {
        //TODO: Decrease complexity
        SwitchParameter help = new("Help", "Prints help information", new []{"-h", "--help"}, args, false);
        if (help.Value)
        {
            PrintHelp();
            return;
        }
        
        Parameter<string> chunkInputPath = new("Input", "Input file with chunk", new []{"-i", "--input"}, args, true);
        Parameter<string> fileInputPath = new("File", "Input file to add the chunk to", new []{"-f", "--file"}, args, true);
        Parameter<string> outputPath = new("Output", "Output file to write", new []{"-o", "--output"}, args, true);
        Parameter<int> chunkPosition = new("Position", "Position of the chunk in the file", new []{"-p", "--position"}, args, false);

        IChunk chunk;
        IRiffFile riffFile;
        
        // Read the chunk file
        {
            var chunkReader = new RiffReader(chunkInputPath.Value, false);
            try
            {
                chunk = chunkReader.ReadChunk();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("There was an error while reading the file with chunk:");
                Console.Error.WriteLine(e.Message);
                return;
            }
        }
        
        // Read the riff file
        {
            var chunkReader = new RiffReader(fileInputPath.Value, false);
            try
            {
                riffFile = chunkReader.ReadFile();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("There was an error while reading the input RIFF file:");
                Console.Error.WriteLine(e.Message);
                return;
            }
        }
        
        int position = chunkPosition.HasValue ? chunkPosition.Value : -1;
        riffFile.MainChunk.InsertChunk(chunk, position);

        try
        {
            var writer = new RiffWriter();
            writer.Write(outputPath.Value, riffFile);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <inheritdoc />
    public override void PrintHelp()
    {
        Console.WriteLine("Adds a chunk to a RIFF file");
        Console.WriteLine("Usage: RiffViewer add [options]");
        Console.WriteLine("\nOptions:");
        Console.WriteLine("\n[Name]\t\t[Required]\t[Flags]\t\t[Description]");
        
        Parameter<string>.PrintHelp("Input", "Input file to read", new []{"-i", "--input"}, true);
        Parameter<string>.PrintHelp("Output", "Output file to write", new []{"-o", "--output"}, true);
    }
}