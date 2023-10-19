using RiffViewer.Lib.Reader;

namespace RiffViewer;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: RiffViewer <path to RIFF file>");
            return;
        }

        string path = args[0];

        if (!File.Exists(path))
        {
            Console.WriteLine($"File at {Path.GetFullPath(path)} doesn't exist!");
            return;
        }

        Console.WriteLine($"Reading file at {Path.GetFullPath(path)}...");

        var reader = new RiffReader(path);

        try
        {
            var riffFile = reader.Read();
            Console.WriteLine(riffFile);
        }
        catch (Exception e)
        {
            Console.WriteLine("There was an error while reading the file:");
            Console.WriteLine(e.Message);
        }
    }
}