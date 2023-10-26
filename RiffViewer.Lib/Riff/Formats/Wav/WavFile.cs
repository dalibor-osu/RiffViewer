using System.Text;
using RiffViewer.Lib.Exceptions;
using RiffViewer.Lib.Riff.Chunk;

namespace RiffViewer.Lib.Riff.Formats.Wav;

public class WavFile : IRiffFile
{
    // <inheritdoc />
    public string Path { get; init; }
    
    // <inheritdoc />
    public RiffChunk MainChunk { get; init; }
    
    public RiffFormat Format { get; init; }
    
    public FmtChunk FmtChunk => GetFmtChunk();
    
    private FmtChunk GetFmtChunk()
    {
        if (MainChunk.ChildChunks.Find(x => x.Identifier == "fmt ") is not FmtChunk fmtChunk)
        {
            throw new RiffFileException("No fmt chunk found in wav file.");
        }
        
        return fmtChunk;
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();

        builder.AppendLine($"Path: {Path}");
        builder.Append(MainChunk.ToString().Replace("\n", "\n\t"));

        return builder.ToString();
    }
}