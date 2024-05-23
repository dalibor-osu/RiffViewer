using System.Globalization;
using System.Text;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Riff.Chunk;

/// <summary>
/// Structure representing a general RIFF chunk and should be inherited
/// by all other chunk classes. Implements <see cref="IChunk"/>.
/// </summary>
public abstract class Chunk : IChunk
{
    /// <inheritdoc />
    public string Identifier { get; }

    /// <inheritdoc />
    public int Length { get; protected set; }

    /// <inheritdoc />
    public long Offset { get; set; }

    /// <inheritdoc />
    public IChunk? ParentChunk { get; set; }

    /// <inheritdoc />
    public bool HasParent => ParentChunk != null;

    /// <summary>
    /// Gets the offset of the data of the chunk from the start of the file in bytes.
    /// </summary>
    public long DataOffset => Offset + CHUNK_HEADER_LENGTH_BYTES;

    /// <summary>
    /// Initializes a new instance of the <see cref="Chunk"/> class.
    /// </summary>
    /// <param name="identifier">Identifier of the chunk... e.g. RIFF, LIST, ...</param>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    /// <param name="parentChunk">Parent chunk or null, if current chunk doesn't have a parent (RIFF chunk only)</param>
    protected Chunk(string identifier, long offset, int length, IChunk? parentChunk)
    {
        Identifier = identifier;
        Offset = offset;
        Length = length;
        ParentChunk = parentChunk;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();

        builder.AppendLine($"\nIdentifier: {Identifier}");
        builder.AppendLine($"Length: {Length.ToString("N0", new CultureInfo("cs-CZ"))} bytes");
        builder.AppendLine($"Offset: {Offset.ToString("N0", new CultureInfo("cs-CZ"))} bytes");

        return builder.ToString();
    }

    /// <inheritdoc />
    public virtual byte[] GetBytes()
    {
        List<byte> bytes = new();
        bytes.AddRange(Encoding.ASCII.GetBytes(Identifier));
        bytes.AddRange(BitConverter.GetBytes(Length));

        return bytes.ToArray();
    }

    /// <inheritdoc />
    public virtual string GetChunkPath()
    {
        if (ParentChunk == null)
        {
            return string.Empty;
        }

        string parentPath = ParentChunk.GetChunkPath();
        
        if (parentPath == string.Empty)
        {
            return Identifier;
        }
        
        return $"{parentPath}.{Identifier}";
    }
}