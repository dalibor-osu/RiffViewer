using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Formats;

namespace RiffViewer.Lib.Riff;

public interface IRiffFile
{
    /// <summary>
    /// Gets the path to the RIFF file.
    /// </summary>
    public string Path { get; }
    
    /// <summary>
    /// Gets the main chunk of the RIFF file.
    /// </summary>
    public RiffChunk MainChunk { get; }
    public RiffFormat Format { get; }
}