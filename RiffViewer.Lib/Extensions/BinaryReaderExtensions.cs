using System.Text;

namespace RiffViewer.Lib.Extensions;

/// <summary>
/// Extension methods for <see cref="BinaryReader"/> used to read RIFF files.
/// </summary>
public static class BinaryReaderExtensions
{
    /// <summary>
    /// Skips the specified number of bytes in the current stream.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> to skip the bytes in</param>
    /// <param name="bytes">Number of bytes to skip</param>
    /// <returns>Instance of a <see cref="BinaryReader"/> passed to this function</returns>
    public static BinaryReader Skip(this BinaryReader reader, long bytes)
    {
        reader.BaseStream.Seek(bytes, SeekOrigin.Current);
        return reader;
    }

    /// <summary>
    /// Goes to the specified position in the current stream.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> to move</param>
    /// <param name="position">Position in the file to go to</param>
    /// <returns>Instance of a <see cref="BinaryReader"/> passed to this function</returns>
    public static BinaryReader GoTo(this BinaryReader reader, long position)
    {
        reader.BaseStream.Seek(position, SeekOrigin.Begin);
        return reader;
    }

    /// <summary>
    /// Reads the next 4 bytes from the current stream as <see cref="string"/> and advances the current position of the stream by 4 bytes.
    /// </summary>
    /// <param name="reader">Instance of <see cref="BinaryReader"/> to read the string from</param>
    /// <returns>Next 4 bytes in the stream as <see cref="string"/></returns>
    public static string Read4ByteString(this BinaryReader reader)
    {
        var bytes = reader.ReadBytes(4);
        return Encoding.ASCII.GetString(bytes);
    }
}