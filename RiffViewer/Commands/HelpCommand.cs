using RiffViewer.Parameters;

namespace RiffViewer.Commands;

/// <summary>
/// Command that prints help information
/// </summary>
public class HelpCommand : Command
{
    /// <inheritdoc />
    protected sealed override string Name => "help";

    /// <inheritdoc />
    protected sealed override string Description => "Prints help information";

    private readonly List<Command> _availableCommands;

    /// <summary>
    /// Constructor for the help command
    /// </summary>
    /// <param name="availableCommands">List of all other available commands</param>
    public HelpCommand(List<Command> availableCommands)
    {
        _availableCommands = availableCommands;
        _availableCommands.Add(this);
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
        SwitchParameter help = new("Help", "Prints help information", new[] { "-h", "--help" }, args, false);
        if (help.Value)
        {
            PrintHelp();
            return;
        }

        Console.WriteLine("Usage: RiffViewer <command> [options]");
        Console.WriteLine("\nAvailable commands:");
        Console.WriteLine("\n[Name]\t\t[Description]");

        foreach (var command in _availableCommands)
        {
            command.Print();
        }

        Console.WriteLine("\nUse RiffViewer <command> --help to get help for a specific command");
    }

    /// <inheritdoc />
    public override void PrintHelp()
    {
        Console.WriteLine("Prints help information");
        Console.WriteLine("Usage: RiffViewer help");
    }
}