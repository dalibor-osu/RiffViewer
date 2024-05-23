using System.Text;

namespace RiffViewer;

// Source: https://stackoverflow.com/a/10533782
public class ConsoleErrorWriterDecorator : TextWriter
{
    private readonly TextWriter _originalConsoleStream;

    private ConsoleErrorWriterDecorator(TextWriter consoleTextWriter)
    {
        _originalConsoleStream = consoleTextWriter;
    }

    public override void WriteLine(string? value)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;

        _originalConsoleStream.WriteLine(value);

        Console.ForegroundColor = originalColor;
    }

    public override Encoding Encoding => Encoding.Default;

    public static void SetToConsole()
    {
        Console.SetError(new ConsoleErrorWriterDecorator(Console.Error));
    }
}