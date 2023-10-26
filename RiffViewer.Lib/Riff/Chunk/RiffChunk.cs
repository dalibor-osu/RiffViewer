using System.Text;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Riff.Chunk;

/// <summary>
/// Structure representing a RIFF chunk.
/// </summary>
public class RiffChunk : Chunk
{
    /// <summary>
    /// Gets the child chunks of this chunk.
    /// </summary>
    public List<IChunk> ChildChunks { get; }

    /// <summary>
    /// Gets the format of this chunk.
    /// </summary>
    public string Format { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiffChunk"/> class with child chunks.
    /// </summary>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    /// <param name="format">Format of the RIFF chunk... e.g. WAVE, AVI, WEBP, ...</param>
    /// <param name="childChunks">Child chunks of this chunks</param>
    public RiffChunk(long offset, int length, string format, List<IChunk> childChunks)
        : base(RIFF_CHUNK_IDENTIFIER, offset, length)
    {
        ChildChunks = childChunks;
        Format = format;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiffChunk"/> class without child chunks.
    /// </summary>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    /// <param name="format">Format of the RIFF chunk... e.g. WAVE, AVI, WEBP, ...</param>
    public RiffChunk(long offset, int length, string format)
        : this(offset, length, format, new List<IChunk>())
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.AppendLine($"Format: {Format}");
        builder.Append("Child Chunks:");

        foreach (var chunk in ChildChunks)
        {
            builder.Append("\n\t--------------------");
            builder.Append(chunk.ToString().Replace("\n", "\n\t"));
        }

        return builder.ToString();
    }
}