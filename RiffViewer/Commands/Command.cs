namespace RiffViewer.Commands;

/// <summary>
/// Abstract class for all commands
/// </summary>
public abstract class Command
{
    /// <summary>
    /// Name of the command
    /// </summary>
    protected abstract string Name { get; }
    
    /// <summary>
    /// Description of the command
    /// </summary>
    protected abstract string Description { get; }
    
    /// <summary>
    /// Invoked when the command is called. This method contains the logic of the command.
    /// </summary>
    /// <param name="args">Program arguments</param>
    public abstract void Execute(string[] args);
    
    /// <summary>
    /// Prints the command name and description
    /// </summary>
    public void Print()
    {
        Console.WriteLine($"{Name}\t\t{Description}");
    }
    
    /// <summary>
    /// Prints the help information for the command
    /// </summary>
    public abstract void PrintHelp();
    
    /// <summary>
    /// Returns the command based on the program arguments
    /// </summary>
    /// <param name="args">Program arguments</param>
    /// <returns><see cref="Command"/> based on the program arguments. If no or unknown arguments are passed, it returns the <see cref="HelpCommand"/></returns>
    public static Command GetCommand(string[] args)
    {
        var command = args.Length < 1 ? "help" : args[0];

        List<Command> commands = new()
        {
            new ReadCommand(),
            new ExtractCommand(),
            new AddCommand(),
            new RemoveCommand()
        };
        
        var helpCommand = new HelpCommand(commands);
        
        return commands.Find(c => c.Name == command) ?? helpCommand;
    }
}