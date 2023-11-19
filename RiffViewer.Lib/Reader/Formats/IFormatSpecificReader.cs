using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;

namespace RiffViewer.Lib.Reader.Formats;

/// <summary>
/// Interface for format specific readers
/// </summary>
public interface IFormatSpecificReader
{
    /// <summary>
    /// Reads the format specific data from the file
    /// </summary>
    /// <param name="path">Path of the RIFF file</param>
    /// <param name="reader">Binary reader with a file stream</param>
    /// <param name="riffChunk">Main chunk of the RIFF file</param>
    /// <returns></returns>
    public IRiffFile ReadFormatSpecificData(string path, BinaryReader reader, RiffChunk riffChunk);
}