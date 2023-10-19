namespace RiffViewer.Lib.Riff;

/// <summary>
/// Class with constants used in the RIFF file format.
/// </summary>
public static class Constants
{
    public const string RIFF_CHUNK_IDENTIFIER = "RIFF";
    public const string LIST_CHUNK_IDENTIFIER = "LIST";
    public const int CHUNK_HEADER_LENGTH_BYTES = 8;
    public const int CHUNK_IDENTIFIER_LENGTH_BYTES = 4;
}