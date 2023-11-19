using RiffViewer.Lib.Reader.Formats;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Formats;
using RiffViewer.Lib.Riff.Formats.Wav;
using static RiffViewer.Lib.Riff.Constants;

namespace RiffViewer.Lib.Helpers;

/// <summary>
/// Helper class for specific formats of a RIFF file
/// </summary>
public static class RiffFormatHelper
{
    /// <summary>
    /// Returns the RIFF format that corresponds to the given string
    /// </summary>
    /// <param name="source"><see cref="string"/> value to find the format from</param>
    /// <returns><see cref="RiffFormat"/> parsed from the <paramref name="source"/></returns>
    public static RiffFormat GetRiffFormatFromString(string source)
    {
        return source switch
        {
            WAVE_FORMAT_IDENTIFIER => RiffFormat.Wav,
            _ => RiffFormat.Other
        };
    }

    /// <summary>
    /// Returns the <see cref="IFormatSpecificReader"/> that corresponds to the given <see cref="RiffFormat"/>
    /// </summary>
    /// <param name="format">Format to get the reader for</param>
    /// <returns>Reader for a specific format as <seealso cref="IFormatSpecificReader"/> or null if the format doesn't have a specific reader</returns>
    public static IFormatSpecificReader? GetFormatSpecificReader(RiffFormat format)
    {
        return format switch
        {
            RiffFormat.Wav => new WavReader(),
            _ => null
        };
    }
}