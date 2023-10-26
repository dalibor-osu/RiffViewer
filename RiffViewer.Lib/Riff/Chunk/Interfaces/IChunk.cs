namespace RiffViewer.Lib.Riff.Chunk.Interfaces;

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
    
    /// <summary>
    /// Gets the offset of the chunk from the start of the file in bytes.
    /// </summary>
    public long Offset { get; }
}