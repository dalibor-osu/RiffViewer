namespace RiffViewer.Parameters;

/// <summary>
/// Parameter that does not have a value but is either present or not. The base is a <see cref="Parameter{T}"/> with a <see cref="bool"/> value
/// </summary>
public class SwitchParameter : Parameter<bool>
{
    /// <summary>
    /// Constructor for the switch parameter
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    /// <param name="description">Description of the parameter</param>
    /// <param name="flags">Flags that can be used to pass the parameter</param>
    /// <param name="args">Program arguments</param>
    /// <param name="defaultValue">Default value of the parameter that gets inverted if the parameter is present in the program arguments</param>
    public SwitchParameter(string name, string description, string[] flags, string[] args, bool defaultValue = false) :
        base(name, description, flags)
    {
        Value = args.Intersect(Flags).Any() ? !defaultValue : defaultValue;
        HasValue = true;
    }
}