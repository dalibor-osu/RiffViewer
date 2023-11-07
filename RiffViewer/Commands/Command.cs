namespace RiffViewer.Commands;

public abstract class Command
{
    protected abstract string Name { get; }
    protected abstract string Description { get; }
    
    public abstract void Execute(string[] args);
    
    public void Print()
    {
        Console.WriteLine($"{Name}\t\t{Description}");
    }
    
    public abstract void PrintHelp();
    
    public static Command GetCommand(string[] args)
    {
        var command = args.Length < 1 ? "help" : args[0];
        
        return command switch
        {
            "read" => new ReadCommand(),
            "help" => new HelpCommand(),
            _ => new HelpCommand()
        };
    }
}