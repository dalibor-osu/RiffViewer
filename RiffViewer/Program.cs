using System.Diagnostics;
using System.Text.Json;
using RiffViewer.Commands;

namespace RiffViewer;

public static class Program
{
    public static void Main(string[] args)
    {
        ConsoleErrorWriterDecorator.SetToConsole();
        var command = Command.GetCommand(args);
#if DEBUG
        var stopWatch = Stopwatch.StartNew();
#endif
        try
        {
            command.Execute(args);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("There was an error while executing the command:");
            Console.Error.WriteLine(e.Message);
        }
#if DEBUG
        finally
        {
            stopWatch.Stop();
            Console.WriteLine($"Execution took {stopWatch.ElapsedMilliseconds}ms");
        }
#endif
    }
}