using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.Lib.Riff.Formats.Wav;

namespace RiffViewer.Lib.Extensions;

/// <summary>
/// Extension methods for <see cref="IDataChunk"/>
/// </summary>
public static class DataChunkExtensions
{
    /// <summary>
    /// Converts a <see cref="IDataChunk"/> to a <see cref="FmtChunk"/>
    /// </summary>
    /// <param name="dataChunk">Chunk to convert to <see cref="FmtChunk"/></param>
    /// <returns>Chunk converted to <see cref="FmtChunk"/></returns>
    public static FmtChunk ToFmtChunk(this IDataChunk dataChunk)
    {
        return new FmtChunk(dataChunk.Identifier, dataChunk.Offset, dataChunk.Length, dataChunk.Data, dataChunk.Loaded);
    }
}