using RiffViewer.Lib.Reader.Formats;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Formats;
using RiffViewer.Lib.Riff.Formats.Wav;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Helpers;

public static class RiffFormatHelper
{
    public static RiffFormat GetRiffFormatFromString(string source)
    {
        return source switch
        {
            WAVE_FORMAT_IDENTIFIER => RiffFormat.Wav,
            _ => RiffFormat.Other
        };
    }
    
    public static IFormatSpecificReader? GetFormatSpecificReader(RiffFormat format)
    {
        return format switch
        {
            RiffFormat.Wav => new WavReader(),
            _ => null
        };
    }
}