using RiffViewer.Parameters;

namespace RiffViewer.Commands;

public class HelpCommand : Command
{
    protected sealed override string Name => "help";
    protected sealed override string Description => "Prints help information";

    public override void Execute(string[] args)
    {
        SwitchParameter help = new("Help", "Prints help information", new []{"-h", "--help"}, args, false);
        if (help.Value)
        {
            PrintHelp();
            return;
        }
        
        List<Command> commands = new()
        {
            new ReadCommand(),
            new HelpCommand()
        };
        
        Console.WriteLine("Usage: RiffViewer <command> [options]");
        Console.WriteLine("\nAvailable commands:");
        Console.WriteLine("\n[Name]\t\t[Description]");
        
        foreach (var command in commands)
        {
            command.Print();
        }
        
        Console.WriteLine("\nUse RiffViewer <command> --help to get help for a specific command");
    }

    public override void PrintHelp()
    {
        Console.WriteLine("Prints help information");
        Console.WriteLine("Usage: RiffViewer help");
    }
}