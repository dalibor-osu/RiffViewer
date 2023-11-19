namespace RiffViewer.Lib.Riff.Chunk.Interfaces;

public interface IDataChunk : IChunk
{
    /// <summary>
    /// Gets whether the data of this chunk is loaded.
    /// </summary>
    public bool Loaded { get; }

    /// <summary>
    /// Gets the data of this chunk.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Sets the data of this chunk.
    /// </summary>
    /// <param name="data">Data to set</param>
    public void SetData(byte[] data);
}