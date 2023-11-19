using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.Lib.Riff.Formats;

namespace RiffViewer.Lib.Riff;

/// <summary>
/// Interface for all RIFF files
/// </summary>
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

    /// <summary>
    /// Format of the RIFF file.
    /// </summary>
    public RiffFormat Format { get; }

    /// <summary>
    /// Finds a sub chunk by its name.
    /// </summary>
    /// <param name="name">Name of a chunk to find</param>
    /// <returns>Chunk as <seealso cref="IChunk"/> or null</returns>
    public IChunk? FindChunk(string name);

    /// <summary>
    /// Removes a chunk from the RIFF file.
    /// </summary>
    /// <param name="name">Name of a chunk to remove</param>
    /// <returns>Size of the removed chunk in bytes or -1 if the chunk was not found</returns>
    public int RemoveChunk(string name);
}