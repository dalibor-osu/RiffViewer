using System.Text;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Riff.Chunk;

public abstract class Chunk : IChunk
{
    public string Identifier { get; }
    public int Length { get; }
    public long Offset { get; }
    public long DataOffset => Offset + CHUNK_HEADER_LENGTH_BYTES;

    public Chunk(string identifier, long offset, int length)
    {
        Identifier = identifier;
        Offset = offset;
        Length = length;
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        
        builder.AppendLine($"\nIdentifier: {Identifier}");
        builder.AppendLine($"Length: {Length} bytes");
        builder.AppendLine($"Offset: {Offset} bytes");
        
        return builder.ToString();
    }
}