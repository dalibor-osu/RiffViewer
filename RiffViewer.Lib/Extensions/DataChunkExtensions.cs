using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.Lib.Riff.Formats.Wav;

namespace RiffViewer.Lib.Extensions;

public static class DataChunkExtensions
{
    public static FmtChunk ToFmtChunk(this IDataChunk dataChunk)
    {
        return new FmtChunk(dataChunk.Identifier, dataChunk.Offset, dataChunk.Length, dataChunk.Data, dataChunk.Loaded);
    }
}