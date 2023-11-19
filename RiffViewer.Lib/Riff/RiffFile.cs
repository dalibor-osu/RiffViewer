using System.Text;
using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.Lib.Riff.Formats;

namespace RiffViewer.Lib.Riff;

/// <summary>
/// Container class for a RIFF file that contains a path to the file and its main chunk.
/// </summary>
public class RiffFile : IRiffFile
{
    /// <inheritdoc />
    public string Path { get; }

    /// <inheritdoc />
    public RiffChunk MainChunk { get; }

    public RiffFormat Format { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiffFile"/> class.
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="mainChunk">Main chunk of the file that contains all data of the file.</param>
    public RiffFile(string path, RiffChunk mainChunk)
    {
        Path = path;
        MainChunk = mainChunk;
        Format = RiffFormat.Other;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiffFile"/> class.
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="mainChunk">Main chunk of the file that contains all data of the file.</param>
    /// <param name="format">Specific format of this file</param>
    public RiffFile(string path, RiffChunk mainChunk, RiffFormat format)
    {
        Path = path;
        MainChunk = mainChunk;
        Format = format;
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