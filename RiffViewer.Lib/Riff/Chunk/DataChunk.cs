using System.Text;
using RiffViewer.Lib.Exceptions;
using RiffViewer.Lib.Riff.Chunk.Interfaces;

namespace RiffViewer.Lib.Riff.Chunk;

/// <summary>
/// Structure representing a general data chunk.
/// </summary>
public class DataChunk : Chunk, IDataChunk
{
    /// <inheritdoc />
    public bool Loaded { get; private set; }

    /// <inheritdoc />
    public byte[] Data { get; private set; }

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

    public void SetData(byte[] data)
    {
        Data = data;
        Loaded = true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.Append($"Loaded: {Loaded}");

        return builder.ToString();
    }

    /// <inheritdoc />
    public override byte[] GetBytes()
    {
        if (!Loaded)
        {
            throw new RiffFileException("Data not loaded!");
        }

        List<byte> bytes = new(base.GetBytes());
        bytes.AddRange(Data);

        return bytes.ToArray();
    }
}