using System.Text;
using RiffViewer.Lib.Exceptions;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Riff.Formats.Wav;

public class FmtChunk : Chunk.Chunk, IDataChunk
{
    /// <inheritdoc />
    public bool Loaded { get; private set; }

    /// <inheritdoc />
    public byte[] Data { get; private set; }

    /// <summary>
    /// Format of the audio data
    /// </summary>
    public int AudioFormat { get; }

    /// <summary>
    /// Number of channels
    /// </summary>
    public int ChannelCount { get; }

    /// <summary>
    /// Sampling rate in Hz
    /// </summary>
    public int SamplingRate { get; }

    /// <summary>
    /// Rate of the data in bytes per second
    /// </summary>
    public int DataRate { get; }

    /// <summary>
    /// Size of a data block in bytes.
    /// </summary>
    public int DataBlockSize { get; }

    /// <summary>
    /// Number of bits per sample
    /// </summary>
    public int BitsPerSample { get; }

    /// <summary>
    /// Constructor for a FMT chunk.
    /// </summary>
    /// <param name="identifier">Identifier of the chunk... e.g. RIFF, LIST, ...</param>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    /// <param name="data">Data of the chunk in bytes</param>
    /// <param name="loaded">Whether the data are loaded or not</param>
    /// <exception cref="RiffFileException">Thrown if the length of a FMT chunk is not 16 bytes</exception>
    public FmtChunk(string identifier, long offset, int length, byte[] data, bool loaded = true) : base(identifier,
        offset, length)
    {
        //TODO: FMT chunk length can be different. see https://www.mmsp.ece.mcgill.ca/Documents/AudioFormats/WAVE/WAVE.html
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

    /// <summary>
    /// Sets the data of the chunk and marks it as loaded.
    /// </summary>
    /// <param name="data">Data to set</param>
    public void SetData(byte[] data)
    {
        Data = data;
        Loaded = true;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override byte[] GetBytes()
    {
        if (!Loaded)
        {
            throw new RiffFileException("Data not loaded!");
        }

        List<byte> bytes = new(base.GetBytes());
        bytes.AddRange(Data);

        return bytes.ToArray();
    }
}