using RiffViewer.Lib.Exceptions;
using RiffViewer.Lib.Extensions;
using RiffViewer.Lib.Helpers;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Reader;

/// <summary>
/// Class used for reading RIFF files.
/// </summary>
public class RiffReader
{
    private readonly string _filePath;
    private readonly bool _lazyLoading;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RiffReader"/> class.
    /// </summary>
    /// <param name="filePath">Path to a RIFF file to read</param>
    /// <param name="lazyLoading">Whether the file should be read with or without data. Default is true (without data)</param>
    public RiffReader(string filePath, bool lazyLoading = true)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("filePath can't be null or empty", nameof(filePath));
        }
        
        _filePath = filePath;
        _lazyLoading = lazyLoading;
    }

    /// <summary>
    /// Reads the content of the RIFF file.
    /// </summary>
    /// <returns>Content of a RIFF file as an instance of <see cref="RiffFile"/></returns>
    /// <exception cref="RiffFileException">Thrown if file is not a RIFF file</exception>
    public IRiffFile ReadFile()
    {
        using var fileStream = File.OpenRead(_filePath);
        using var reader = new BinaryReader(fileStream);

        var identifier = reader.Read4ByteString();

        if (identifier != RIFF_CHUNK_IDENTIFIER)
        {
            throw new RiffFileException("File doesn't start with RIFF bytes.");
        }

        var mainChunk = ReadRiffChunk(reader);
        var format = RiffFormatHelper.GetRiffFormatFromString(mainChunk.Format);
        var formatReader = RiffFormatHelper.GetFormatSpecificReader(format);

        if (formatReader != null)
        {
            return formatReader.ReadFormatSpecificData(_filePath, reader, mainChunk);
        }

        return new RiffFile(_filePath, mainChunk);
    }

    /// <summary>
    /// Reads the content of a chunk from a file
    /// </summary>
    /// <returns>Chunk as <see cref="IChunk"/></returns>
    public IChunk ReadChunk()
    {
        using var fileStream = File.OpenRead(_filePath);
        using var reader = new BinaryReader(fileStream);

        return ReadChunkFromFile(reader);
    }
    
    /// <summary>
    /// Reads the content of a chunk.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> used for reading bytes in the current file</param>
    /// <returns>Content of a chunk as <see cref="Chunk"/></returns>
    private IChunk ReadChunkFromFile(BinaryReader reader)
    {
        string chunkIdentifier = reader.Read4ByteString();

        if (chunkIdentifier == RIFF_CHUNK_IDENTIFIER)
        {
            return ReadRiffChunk(reader);
        }
        
        return chunkIdentifier switch
        {
            RIFF_CHUNK_IDENTIFIER => ReadRiffChunk(reader),
            LIST_CHUNK_IDENTIFIER => ReadListChunk(reader, null!),
            _ => ReadDataChunk(reader, chunkIdentifier, null!)
        };
    }

    /// <summary>
    /// Reads the content of a chunk.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> used for reading bytes in the current file</param>
    /// <param name="parentChunk">Parent chunk of the chunk to be read</param>
    /// <returns>Content of a chunk as <see cref="Chunk"/></returns>
    private IChunk ReadChunk(BinaryReader reader, IChunk? parentChunk)
    {
        string chunkIdentifier = reader.Read4ByteString();

        if (chunkIdentifier == RIFF_CHUNK_IDENTIFIER)
        {
            return ReadRiffChunk(reader);
        }

        if (parentChunk == null)
        {
            throw new RiffFileException("Non RIFF chunk must have a non null parent");
        }
        
        return chunkIdentifier switch
        {
            RIFF_CHUNK_IDENTIFIER => ReadRiffChunk(reader),
            LIST_CHUNK_IDENTIFIER => ReadListChunk(reader, parentChunk),
            _ => ReadDataChunk(reader, chunkIdentifier, parentChunk)
        };
    }

    /// <summary>
    /// Reads the content of a data chunk.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> used for reading bytes in the current file</param>
    /// <param name="chunkIdentifier">Identifier of the chunk... e.g. RIFF, LIST, ...</param>
    /// <param name="parentChunk">Parent chunk of the chunk to be read</param>
    /// <param name="position">If set, reader will start reading the content of the chunk from this position in the file</param>
    /// <returns>Content of a data chunk as <see cref="DataChunk"/></returns>
    private IDataChunk ReadDataChunk(BinaryReader reader, string chunkIdentifier, IChunk parentChunk, long position = -1)
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
            return new DataChunk(chunkIdentifier, offset, length, parentChunk);
        }

        byte[] data = reader.ReadBytes(length);
        return new DataChunk(chunkIdentifier, offset, length, parentChunk, data);
    }

    /// <summary>
    /// Reads the content of a list chunk.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> used for reading bytes in the current file</param>
    /// <param name="parentChunk">Parent chunk of the chunk to be read</param>
    /// <param name="position">If set, reader will start reading the content of the chunk from this position in the file</param>
    /// <returns>Content of a list chunk as <see cref="ListChunk"/></returns>
    private ListChunk ReadListChunk(BinaryReader reader, IChunk parentChunk, long position = -1)
    {
        if (position > -1)
        {
            reader.GoTo(position);
        }

        long offset = reader.BaseStream.Position - CHUNK_IDENTIFIER_LENGTH_BYTES;
        int length = reader.ReadInt32();
        string type = reader.Read4ByteString();
        
        var currentChunk = new ListChunk(offset, length, type, parentChunk);
        var childChunks = new List<IChunk>();
        
        long chunkEnd = offset + length + (length % 2 == 0 ? 0 : 1);
        while (reader.BaseStream.Position < chunkEnd)
        {
            var child = ReadChunk(reader, currentChunk);
            if (child.Length % 2 != 0)
            {
                // Skip padding
                reader.ReadByte();
            }
            childChunks.Add(child);
        }

        currentChunk.SetChildChunks(childChunks);
        
        return currentChunk;
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

        var currentChunk = new RiffChunk(offset, length, type);
        var childChunks = new List<IChunk>();

        long chunkEnd = offset + length + (length % 2 == 0 ? 0 : 1);
        while (reader.BaseStream.Position < chunkEnd)
        {
            var child = ReadChunk(reader, currentChunk);
            if (child.Length % 2 != 0)
            {
                // Skip padding
                reader.ReadByte();
            }
            childChunks.Add(child);
        }
        
        currentChunk.SetChildChunks(childChunks);
        
        return currentChunk;
    }
}