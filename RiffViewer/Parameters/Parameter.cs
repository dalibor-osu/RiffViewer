namespace RiffViewer.Parameters;

/// <summary>
/// Base class for a command parameter
/// </summary>
/// <typeparam name="T">Type of the passed value</typeparam>
public class Parameter<T>
{
    /// <summary>
    /// Name of the parameter
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Description of the parameter
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Flags that can be used to pass the parameter
    /// </summary>
    public string[] Flags { get; private set; }

    /// <summary>
    /// Whether the parameter is required
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// Whether the parameter has a value
    /// </summary>
    public bool HasValue { get; protected set; }

    /// <summary>
    /// Value of the parameter
    /// </summary>
    public T Value { get; protected set; } = default!;

    /// <summary>
    /// Constructor for the parameter which also tries to find the value in program arguments
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    /// <param name="description">Description of the parameter</param>
    /// <param name="flags">Flags that can be used to pass the parameter</param>
    /// <param name="args">Program arguments</param>
    /// <param name="isRequired">Whether the parameter is required</param>
    public Parameter(string name, string description, string[] flags, string[] args, bool isRequired = false)
    {
        Name = name;
        Description = description;
        Flags = flags;
        IsRequired = isRequired;

        FindValue(args);
    }

    /// <summary>
    /// Constructor for the parameter. This constructor does not try to find the value in program arguments
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    /// <param name="description">Description of the parameter</param>
    /// <param name="flags">Flags that can be used to pass the parameter</param>
    /// <param name="isRequired">Whether the parameter is required</param>
    protected Parameter(string name, string description, string[] flags, bool isRequired = false)
    {
        Name = name;
        Description = description;
        Flags = flags;
        IsRequired = isRequired;
    }

    /// <summary>
    /// Tries to find the value in program arguments
    /// </summary>
    /// <param name="args">Program arguments</param>
    /// <exception cref="ArgumentException">Thrown if the parameter is required but it was not found in the program arguments</exception>
    private void FindValue(string[] args)
    {
        var intersection = args.Intersect(Flags).ToArray();
        if (!intersection.Any())
        {
            if (IsRequired)
            {
                throw new ArgumentException("Required parameter not found: " + Name);
            }

            return;
        }

        if (intersection.Length > 1)
        {
            throw new ArgumentException("Multiple values found for parameter: " + Name);
        }

        int index = Array.IndexOf(args, intersection[0]) + 1;

        try
        {
            Value = (T)Convert.ChangeType(args[index], typeof(T));
            HasValue = true;
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Error while parsing value for parameter {Name}: {e.Message}");
        }
    }

    /// <summary>
    /// Prints the help for the parameter
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    /// <param name="description">Description of the parameter</param>
    /// <param name="flags">Flags that can be used to pass the parameter</param>
    /// <param name="isRequired">Whether the parameter is required</param>
    public static void PrintHelp(string name, string description, string[] flags, bool isRequired = false)
    {
        Console.WriteLine($"{name}\t\t{isRequired}\t\t{string.Join(", ", flags)}\t{description}");
    }
}