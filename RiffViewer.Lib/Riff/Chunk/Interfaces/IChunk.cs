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

    /// <summary>
    /// Gets the data of the chunk in bytes.
    /// </summary>
    /// <returns></returns>
    public byte[] GetBytes();
    
    /// <summary>
    /// Instance of a parent chunk or null
    /// </summary>
    public IChunk? ParentChunk { get; }

    /// <summary>
    /// Information that says if current chunk has a parent or not
    /// </summary>
    public bool HasParent { get; }

    /// <summary>
    /// Returns path to this chunk
    /// </summary>
    /// <returns></returns>
    public string GetChunkPath();
}