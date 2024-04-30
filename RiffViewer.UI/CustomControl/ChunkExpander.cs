using Avalonia.Controls;
using RiffViewer.Lib.Riff.Chunk.Interfaces;

namespace RiffViewer.UI.CustomControl;

public class ChunkExpander : Expander
{
    public IChunk Chunk { get; set; }
}