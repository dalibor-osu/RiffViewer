using RiffViewer.Lib.Extensions;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Reader;

public class RiffReader
{
    private string _filePath;
    private bool _lazyLoading = true;

    public RiffReader(string filePath)
    {
        _filePath = filePath;
    }

    public RiffFile Read()
    {
        using var fileStream = File.OpenRead(_filePath);
        using var reader = new BinaryReader(fileStream);

        var identifier = reader.Read4ByteString();

        if (identifier != RIFF_CHUNK_IDENTIFIER)
        {
            throw new ArgumentException("File doesn't start with RIFF bytes.");
        }
        
        var mainChunk = ReadRiffChunk(reader);

        return new RiffFile(_filePath, mainChunk);
    }

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