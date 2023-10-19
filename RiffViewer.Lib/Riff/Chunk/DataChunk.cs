using System.Text;

namespace RiffViewer.Lib.Riff.Chunk;

/// <summary>
/// Structure representing a general data chunk.
/// </summary>
public class DataChunk : Chunk
{
    /// <summary>
    /// Gets whether the data of this chunk is loaded.
    /// </summary>
    public bool Loaded { get; }

    /// <summary>
    /// Gets the data of this chunk.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataChunk"/> class with loaded data.
    /// </summary>
    /// <param name="identifier">Identifier of the chunk... e.g. data (for WAVE file)</param>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    /// <param name="data">Data store in this chunk as byte array</param>
    /// <param name="loaded">Indicates if data is loaded</param>
    public DataChunk(string identifier, long offset, int length, byte[] data, bool loaded = true)
        : base(identifier, offset, length)
    {
        Loaded = loaded;
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataChunk"/> class without loaded data.
    /// Automatically sets <see cref="Loaded"/> to false and <see cref="Data"/> to an empty byte array.
    /// </summary>
    /// <param name="identifier">Identifier of the chunk... e.g. data (for WAVE file)</param>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    public DataChunk(string identifier, long offset, int length)
        : this(identifier, offset, length, Array.Empty<byte>(), false)
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.Append($"Loaded: {Loaded}");

        return builder.ToString();
    }
}