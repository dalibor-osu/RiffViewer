using System.Text;
using RiffViewer.Lib.Riff.Chunk;

namespace RiffViewer.Lib.Riff;

public class RiffFile
{
    public string Path { get; set; }
    public RiffChunk MainChunk { get; set; }

    public RiffFile(string path, RiffChunk mainChunk)
    {
        Path = path;
        MainChunk = mainChunk;
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        
        builder.AppendLine($"Path: {Path}");
        builder.Append(MainChunk.ToString().Replace("\n", "\n\t"));

        return builder.ToString();
    }
}