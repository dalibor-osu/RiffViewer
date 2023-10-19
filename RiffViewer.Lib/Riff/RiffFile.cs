using System.Text;
using RiffViewer.Lib.Riff.Chunk;

namespace RiffViewer.Lib.Riff;

/// <summary>
/// Container class for a RIFF file that contains a path to the file and its main chunk.
/// </summary>
public class RiffFile
{
    /// <summary>
    /// Gets the path to the RIFF file.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the main chunk of the RIFF file.
    /// </summary>
    public RiffChunk MainChunk { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiffFile"/> class.
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="mainChunk">Main chunk of the file that contains all data of the file.</param>
    public RiffFile(string path, RiffChunk mainChunk)
    {
        Path = path;
        MainChunk = mainChunk;
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