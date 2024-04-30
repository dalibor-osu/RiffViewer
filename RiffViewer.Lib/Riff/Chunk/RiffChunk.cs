using System.Text;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Riff.Chunk;

/// <summary>
/// Structure representing a RIFF chunk.
/// </summary>
public class RiffChunk : Chunk
{
    /// <summary>
    /// Gets the child chunks of this chunk.
    /// </summary>
    public List<IChunk> ChildChunks { get; private set; }

    /// <summary>
    /// Gets the format of this chunk.
    /// </summary>
    public string Format { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RiffChunk"/> class with child chunks.
    /// </summary>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    /// <param name="format">Format of the RIFF chunk... e.g. WAVE, AVI, WEBP, ...</param>
    /// <param name="childChunks">Child chunks of this chunks</param>
    public RiffChunk(long offset, int length, string format, List<IChunk> childChunks)
        : base(RIFF_CHUNK_IDENTIFIER, offset, length, null)
    {
        ChildChunks = childChunks;
        Format = format;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiffChunk"/> class without child chunks.
    /// </summary>
    /// <param name="offset">Offset from the start of the file in bytes</param>
    /// <param name="length">Length of the chunk in bytes</param>
    /// <param name="format">Format of the RIFF chunk... e.g. WAVE, AVI, WEBP, ...</param>
    internal RiffChunk(long offset, int length, string format)
        : this(offset, length, format, new List<IChunk>())
    {
    }
    
    internal void SetChildChunks(List<IChunk> children)
    {
        ChildChunks = children;
    }

    public IChunk? FindSubChunk(string name)
    {
        string[] names = name.Split('.');
        IChunk? chunk;

        if (names.Length == 1)
        {
            chunk = ChildChunks.Find(c => c.Identifier == names[0]);
            return chunk;
        }

        chunk = ChildChunks.Where(c => c is ListChunk).Cast<ListChunk>().ToList().Find(c => c.Type == names[0]);

        if (chunk is ListChunk listChunk)
        {
            return listChunk.FindSubChunk(string.Join('.', names[1..]));
        }

        return null;
    }

    public void InsertChunk(IChunk chunk, int position)
    {
        if (position < 0 || position > ChildChunks.Count)
        {
            Console.WriteLine("Invalid position for inserting chunk. It will be inserted at the end of the file.");
            ChildChunks.Add(chunk);
        }
        else
        {
            ChildChunks.Insert(position, chunk);
        }

        Length += chunk.Length + CHUNK_HEADER_LENGTH_BYTES;
    }

    public int RemoveChunk(string name)
    {
        //TODO: Create an abstraction for this (ewww)
        string[] names = name.Split('.');
        IChunk? chunk;

        if (names.Length == 1)
        {
            chunk = ChildChunks.Find(c => c.Identifier == names[0]) ??
                    ChildChunks.Where(c => c is ListChunk).Cast<ListChunk>().ToList().Find(c => c.Type == names[0]);

            if (chunk == null)
            {
                return -1;
            }

            ChildChunks.Remove(chunk);
            Length -= chunk.Length + CHUNK_HEADER_LENGTH_BYTES;
            return chunk.Length;
        }

        chunk = ChildChunks.Where(c => c is ListChunk).Cast<ListChunk>().ToList().Find(c => c.Type == names[0]);

        if (chunk is not ListChunk listChunk)
        {
            return -1;
        }

        int size = listChunk.RemoveChunk(string.Join('.', names[1..]));

        if (size < 0)
        {
            return -1;
        }

        Length -= size + CHUNK_HEADER_LENGTH_BYTES;
        return size;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString());
        builder.AppendLine($"Format: {Format}");
        builder.Append("Child Chunks:");

        foreach (var chunk in ChildChunks)
        {
            builder.Append("\n\t--------------------");
            builder.Append(chunk.ToString()?.Replace("\n", "\n\t"));
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public override byte[] GetBytes()
    {
        List<byte> bytes = new();
        bytes.AddRange(Encoding.ASCII.GetBytes(Identifier));
        bytes.AddRange(BitConverter.GetBytes(Length));
        bytes.AddRange(Encoding.ASCII.GetBytes(Format));

        foreach (var chunk in ChildChunks)
        {
            bytes.AddRange(chunk.GetBytes());
        }

        return bytes.ToArray();
    }

    public override string GetChunkPath()
    {
        return string.Empty;
    }
}