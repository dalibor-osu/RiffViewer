namespace RiffViewer.Lib.Riff.Chunk;

/// <summary>
/// Base interface for all RIFF file chunks.
/// </summary>
public interface IChunk
{
    /// <summary>
    /// Gets the identifier of the chunk... e.g. RIFF, LIST, ...
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the length of the chunk in bytes.
    /// </summary>
    public int Length { get; }
}