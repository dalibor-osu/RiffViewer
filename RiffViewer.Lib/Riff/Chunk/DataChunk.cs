using System.Text;

namespace RiffViewer.Lib.Riff.Chunk;

public class DataChunk : Chunk
{
    public bool Loaded { get; }
    public byte[] Data { get; }
    
    public DataChunk(string identifier, long offset, int length, byte[] data, bool loaded = true) : base(identifier, offset, length)
    {
        Loaded = loaded;
        Data = data;
    }
    
    public DataChunk(string identifier, long offset, int length) : this(identifier, offset, length, Array.Empty<byte>(), false)
    { }

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.Append($"Loaded: {Loaded}");

        return builder.ToString();
    }
}