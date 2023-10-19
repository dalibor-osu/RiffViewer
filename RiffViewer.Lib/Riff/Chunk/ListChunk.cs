using System.Text;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Riff.Chunk;

/// <summary>
/// Structure representing a LIST chunk.
/// </summary>
public class ListChunk : Chunk
{
    /// <summary>
    /// Gets the child chunks of this chunk.
    /// </summary>
    public List<Chunk> ChildChunks { get; }

    /// <summary>
    /// Gets the type of this chunk.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListChunk"/> class with child chunks.
    /// </summary>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    /// <param name="type">Type of the RIFF chunk... e.g. INFO</param>
    /// <param name="childChunks">Child chunks of this chunks</param>
    public ListChunk(long offset, int length, string type, List<Chunk> childChunks)
        : base(LIST_CHUNK_IDENTIFIER, offset, length)
    {
        ChildChunks = childChunks;
        Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListChunk"/> class without child chunks.
    /// </summary>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    /// <param name="type">Type of the RIFF chunk... e.g. INFO</param>
    public ListChunk(long offset, int length, string type)
        : this(offset, length, type, new List<Chunk>())
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.AppendLine($"Type: {Type}");
        builder.Append($"Child Chunks:");

        foreach (var chunk in ChildChunks)
        {
            builder.Append("\n\t--------------------");
            builder.Append(chunk.ToString().Replace("\n", "\n\t"));
        }

        return builder.ToString();
    }
}