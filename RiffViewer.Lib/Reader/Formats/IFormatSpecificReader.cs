using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;

namespace RiffViewer.Lib.Reader.Formats;

public interface IFormatSpecificReader
{
    public IRiffFile ReadFormatSpecificData(string path, BinaryReader reader, RiffChunk riffChunk);
}