namespace RiffViewer.Parameters;

public class Parameter<T>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string[] Flags { get; private set; }
    public bool IsRequired { get; init; }
    public bool HasValue { get; protected set; }
    public T Value { get; protected set; } = default!;
    
    public Parameter(string name, string description, string[] flags, string[] args, bool isRequired = false)
    {
        Name = name;
        Description = description;
        Flags = flags;
        IsRequired = isRequired;
        
        FindValue(args);
    }
    
    protected Parameter(string name, string description, string[] flags, bool isRequired = false)
    {
        Name = name;
        Description = description;
        Flags = flags;
        IsRequired = isRequired;
    }
    
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
            Value = (T) Convert.ChangeType(args[index], typeof(T));
            HasValue = true;
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Error while parsing value for parameter {Name}: {e.Message}");
        }
    }
    
    public void Print()
    {
        Console.WriteLine($"{Name}\t\t{IsRequired}\t\t{string.Join(", ", Flags)}\t\t{Description}");
    }
    
    public static void PrintHelp(string name, string description, string[] flags, bool isRequired = false)
    {
        
        Console.WriteLine($"{name}\t\t{isRequired}\t\t{string.Join(", ", flags)}\t{description}");
    }
}