using System.Text;

namespace RiffViewer.Lib.Extensions;

public static class BinaryReaderExtensions
{
    public static BinaryReader Skip(this BinaryReader reader, long bytes)
    {
        reader.BaseStream.Seek(bytes, SeekOrigin.Current);
        return reader;
    }

    public static BinaryReader GoTo(this BinaryReader reader, long position)
    {
        reader.BaseStream.Seek(position, SeekOrigin.Begin);
        return reader;
    }

    public static string Read4ByteString(this BinaryReader reader)
    {
        var bytes = reader.ReadBytes(4);
        return Encoding.ASCII.GetString(bytes);
    }
}