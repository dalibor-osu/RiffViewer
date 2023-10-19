using RiffViewer.Lib.Exceptions;
using RiffViewer.Lib.Extensions;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Reader;

/// <summary>
/// Class used for reading RIFF files.
/// </summary>
public class RiffReader
{
    private readonly string _filePath;
    private readonly bool _lazyLoading = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="RiffReader"/> class.
    /// </summary>
    /// <param name="filePath">Path to a RIFF file to read</param>
    public RiffReader(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Reads the content of the RIFF file.
    /// </summary>
    /// <returns>Content of a RIFF file as an instance of <see cref="RiffFile"/></returns>
    /// <exception cref="RiffFileException">Thrown if file is not a RIFF file</exception>
    public RiffFile Read()
    {
        using var fileStream = File.OpenRead(_filePath);
        using var reader = new BinaryReader(fileStream);

        var identifier = reader.Read4ByteString();

        if (identifier != RIFF_CHUNK_IDENTIFIER)
        {
            throw new RiffFileException("File doesn't start with RIFF bytes.");
        }

        var mainChunk = ReadRiffChunk(reader);

        return new RiffFile(_filePath, mainChunk);
    }

    /// <summary>
    /// Reads the content of a chunk.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> used for reading bytes in the current file</param>
    /// <returns>Content of a chunk as <see cref="Chunk"/></returns>
    private Chunk ReadChunk(BinaryReader reader)
    {
        string chunkIdentifier = reader.Read4ByteString();

        return chunkIdentifier switch
        {
            RIFF_CHUNK_IDENTIFIER => ReadRiffChunk(reader),
            LIST_CHUNK_IDENTIFIER => ReadListChunk(reader),
            _ => ReadDataChunk(reader, chunkIdentifier)
        };
    }

    /// <summary>
    /// Reads the content of a data chunk.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> used for reading bytes in the current file</param>
    /// <param name="chunkIdentifier">Identifier of the chunk... e.g. RIFF, LIST, ...</param>
    /// <param name="position">If set, reader will start reading the content of the chunk from this position in the file</param>
    /// <returns>Content of a data chunk as <see cref="DataChunk"/></returns>
    private DataChunk ReadDataChunk(BinaryReader reader, string chunkIdentifier, long position = -1)
    {
        if (position > -1)
        {
            reader.GoTo(position);
        }

        long offset = reader.BaseStream.Position - CHUNK_IDENTIFIER_LENGTH_BYTES;
        int length = reader.ReadInt32();

        if (_lazyLoading)
        {
            reader.Skip(length);
            return new DataChunk(chunkIdentifier, offset, length);
        }

        byte[] data = reader.ReadBytes(length);
        return new DataChunk(chunkIdentifier, offset, length, data);
    }

    /// <summary>
    /// Reads the content of a list chunk.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> used for reading bytes in the current file</param>
    /// <param name="position">If set, reader will start reading the content of the chunk from this position in the file</param>
    /// <returns>Content of a list chunk as <see cref="ListChunk"/></returns>
    private ListChunk ReadListChunk(BinaryReader reader, long position = -1)
    {
        if (position > -1)
        {
            reader.GoTo(position);
        }

        long offset = reader.BaseStream.Position - CHUNK_IDENTIFIER_LENGTH_BYTES;
        int length = reader.ReadInt32();
        string type = reader.Read4ByteString();
        var childChunks = new List<Chunk>();

        long chunkEnd = offset + length + (length % 2 == 0 ? 0 : 1);
        while (reader.BaseStream.Position < chunkEnd)
        {
            childChunks.Add(ReadChunk(reader));
        }

        return new ListChunk(offset, length, type, childChunks);
    }

    /// <summary>
    /// Reads the content of a riff chunk.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> used for reading bytes in the current file</param>
    /// <param name="position">If set, reader will start reading the content of the chunk from this position in the file</param>
    /// <returns>Content of a riff chunk as <see cref="RiffChunk"/></returns>
    private RiffChunk ReadRiffChunk(BinaryReader reader, long position = -1)
    {
        if (position >= 0)
        {
            reader.GoTo(position);
        }

        long offset = reader.BaseStream.Position - CHUNK_IDENTIFIER_LENGTH_BYTES;
        int length = reader.ReadInt32();
        string type = reader.Read4ByteString();
        var childChunks = new List<Chunk>();

        long chunkEnd = offset + length + (length % 2 == 0 ? 0 : 1);
        while (reader.BaseStream.Position < chunkEnd)
        {
            childChunks.Add(ReadChunk(reader));
        }

        return new RiffChunk(offset, length, type, childChunks);
    }
}