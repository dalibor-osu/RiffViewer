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