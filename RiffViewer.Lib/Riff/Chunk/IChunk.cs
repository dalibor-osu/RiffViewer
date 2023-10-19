namespace RiffViewer.Lib.Riff.Chunk;

public interface IChunk
{
    public string Identifier { get; }
    public int Length { get; }
}