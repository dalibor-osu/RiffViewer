using System.Text;
using RiffViewer.Lib.Exceptions;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Riff.Formats.Wav;

public class FmtChunk : Chunk.Chunk, IDataChunk
{
    // <inheritdoc />
    public bool Loaded { get; private set; }
    
    // <inheritdoc />
    public byte[] Data { get; private set; }
    
    public int AudioFormat { get; }
    public int ChannelCount { get; }
    public int SamplingRate { get; }
    public int DataRate { get; }
    public int DataBlockSize { get; }
    public int BitsPerSample { get; }

    public FmtChunk(string identifier, long offset, int length, byte[] data, bool loaded = true) : base(identifier, offset, length)
    {
        if (length != FMT_CHUNK_LENGTH_BYTES)
        {
            throw new RiffFileException("fmt chunk length is not 16 bytes.");
        }
        
        var dataSpan = data.AsSpan();
        AudioFormat = BitConverter.ToInt16(dataSpan[..2]);
        ChannelCount = BitConverter.ToInt16(dataSpan[2..4]);
        SamplingRate = BitConverter.ToInt32(dataSpan[4..8]);
        DataRate = BitConverter.ToInt32(dataSpan[8..12]);
        DataBlockSize = BitConverter.ToInt16(dataSpan[12..14]);
        BitsPerSample = BitConverter.ToInt16(dataSpan[14..16]);
        
        Loaded = loaded;
        Data = data;
    }
    
    public void SetData(byte[] data)
    {
        Data = data;
        Loaded = true;
    }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append(base.ToString());
        builder.AppendLine($"\nAudio Format: {AudioFormat}");
        builder.AppendLine($"Channel Count: {ChannelCount}");
        builder.AppendLine($"Sampling Rate: {SamplingRate}");
        builder.AppendLine($"Data Rate: {DataRate}");
        builder.AppendLine($"Data Block Size: {DataBlockSize}");
        builder.Append($"Bits Per Sample: {BitsPerSample}");

        return builder.ToString();
    }
}