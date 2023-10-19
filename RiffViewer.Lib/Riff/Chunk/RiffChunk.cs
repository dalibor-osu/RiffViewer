using System.Text;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Riff.Chunk;

public class RiffChunk : Chunk
{
    public List<Chunk> ChildChunks { get; set; }
    public string Type { get; }
    
    public RiffChunk(long offset, int length, string type, List<Chunk> childChunks) : base(RIFF_CHUNK_IDENTIFIER, offset, length)
    {
        ChildChunks = childChunks;
        Type = type;
    }
    
    public RiffChunk(long offset, int length, string type) : this(offset, length, type, new List<Chunk>())
    { }
    
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.AppendLine($"Type: {Type}");
        builder.Append("Child Chunks:");
        
        foreach (var chunk in ChildChunks)
        {
            builder.Append("\n\t--------------------");
            builder.Append(chunk.ToString().Replace("\n", "\n\t"));
        }

        return builder.ToString();
    }
}