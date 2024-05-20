using System.Text;
using RiffViewer.Lib.Exceptions;
using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Chunk.Interfaces;

namespace RiffViewer.Lib.Riff.Formats.Wav;

public class WavFile : IRiffFile
{
    /// <inheritdoc />
    public string Path { get; init; } = string.Empty;

    /// <inheritdoc />
    public RiffChunk MainChunk { get; init; } = null!;

    /// <inheritdoc />
    public RiffFormat Format { get; init; }

    /// <summary>
    /// FMT chunk of the wav file.
    /// </summary>
    public FmtChunk FmtChunk => GetFmtChunk();

    /// <summary>
    /// Returns the FMT chunk of the wav file.
    /// </summary>
    /// <returns>FMT chunk as <see cref="FmtChunk"/></returns>
    /// <exception cref="RiffFileException">Thrown if the file doesn't contain a FMT file, thus is probably corrupted</exception>
    private FmtChunk GetFmtChunk()
    {
        if (MainChunk.ChildChunks.Find(x => x.Identifier == "fmt ") is not FmtChunk fmtChunk)
        {
            throw new RiffFileException("No fmt chunk found in wav file.");
        }

        return fmtChunk;
    }

    /// <summary>
    /// Replaces a FMT chunk of a wav file with a new one
    /// </summary>
    /// <param name="fmtChunk">New FMT chunk to use</param>
    /// <exception cref="RiffFileException">Thrown if the file doesn't contain a FMT file, thus is probably corrupted</exception>
    public void SetFmtChunk(FmtChunk fmtChunk)
    {
        int fmtChunkIndex = MainChunk.ChildChunks.FindIndex(x => x.Identifier == "fmt ");
        if (fmtChunkIndex < 0)
        {
            throw new RiffFileException("No fmt chunk found in wav file.");
        }

        MainChunk.ChildChunks[fmtChunkIndex] = fmtChunk;
    }

    public double GetLengthInSeconds()
    {
        if (FindChunk("data") is not IDataChunk dataChunk)
        {
            return -1;
        }
        
        var time = 1.0 / FmtChunk.SamplingRate;
        return dataChunk.Data.Length / (double)FmtChunk.DataBlockSize * time;
    }

    /// <inheritdoc />
    public IChunk? FindChunk(string name)
    {
        return MainChunk.FindSubChunk(name);
    }

    /// <inheritdoc />
    public int RemoveChunk(string name)
    {
        return MainChunk.RemoveChunk(name);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();

        builder.AppendLine($"Path: {Path}");
        builder.Append(MainChunk.ToString().Replace("\n", "\n\t"));

        return builder.ToString();
    }
}