using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk.Interfaces;

namespace RiffViewer.Lib.Writer;

/// <summary>
/// Class for writing RIFF files
/// </summary>
public class RiffWriter
{
    /// <summary>
    /// Writes a RIFF file to file in the specified path
    /// </summary>
    /// <param name="path">Path to write the data to</param>
    /// <param name="file">RIFF file to write</param>
    public void Write(string path, IRiffFile file)
    {
        var fileStream = InitializeFileStream(path);

        using var writer = new BinaryWriter(fileStream);
        writer.Write(file.MainChunk.GetBytes());
        writer.Close();
    }

    /// <summary>
    /// Writes chunk of a RIFF file to file in the specified path
    /// </summary>
    /// <param name="path">Path to write the data to</param>
    /// <param name="chunk">Chunk to write</param>
    public void WriteChunk(string path, IChunk chunk)
    {
        var fileStream = InitializeFileStream(path);

        using var writer = new BinaryWriter(fileStream);
        writer.Write(chunk.GetBytes());
        writer.Close();
    }

    /// <summary>
    /// Initializes a file stream for writing. If the file exists, it will be overwritten. If not, it will be created.
    /// </summary>
    /// <param name="path">Path to file to initialize the stream for</param>
    /// <returns><see cref="FileStream"/> for a given file</returns>
    private FileStream InitializeFileStream(string path)
    {
        FileStream fileStream;
        if (File.Exists(path))
        {
            fileStream = File.Create(path);
        }
        else
        {
            File.WriteAllBytes(path, Array.Empty<byte>());
            fileStream = File.OpenWrite(path);
        }

        return fileStream;
    }
}