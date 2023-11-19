using RiffViewer.Lib.Exceptions;
using RiffViewer.Lib.Extensions;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.Lib.Riff.Formats;
using RiffViewer.Lib.Riff.Formats.Wav;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Reader.Formats;

/// <summary>
/// Format specific reader for WAVE files
/// </summary>
public class WavReader : IFormatSpecificReader
{
    /// <inheritdoc />
    public IRiffFile ReadFormatSpecificData(string path, BinaryReader reader, RiffChunk riffChunk)
    {
        var chunk = riffChunk.ChildChunks.Find(c => c.Identifier == FMT_CHUNK_IDENTIFIER);
        int fmtChunkIndex = riffChunk.ChildChunks.FindIndex(c => c.Identifier == FMT_CHUNK_IDENTIFIER);

        if (chunk == null)
        {
            Console.WriteLine("WAVE file doesn't contain fmt chunk... It might be corrupted.");
            return new WavFile
            {
                Path = path,
                MainChunk = riffChunk,
                Format = RiffFormat.Wav
            };
        }

        if (chunk is not IDataChunk dataChunk)
        {
            throw new RiffFileException(
                "There was an error when reading a WAVE file. fmt chunk is most likely corrupted.");
        }

        var fmtChunk = reader.ReadChunkData(dataChunk).ToFmtChunk();
        riffChunk.ChildChunks[fmtChunkIndex] = fmtChunk;

        return new WavFile
        {
            Path = path,
            MainChunk = riffChunk,
            Format = RiffFormat.Wav
        };
    }
}