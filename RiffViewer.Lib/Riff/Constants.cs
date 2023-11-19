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

    public const string WAVE_FORMAT_IDENTIFIER = "WAVE";
    public const string FMT_CHUNK_IDENTIFIER = "fmt ";
    public const int FMT_CHUNK_LENGTH_BYTES = 16;

    public const string AVI_FORMAT_IDENTIFIER = "AVI ";
}