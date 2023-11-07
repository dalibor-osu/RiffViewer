namespace RiffViewer.Parameters;

public class SwitchParameter : Parameter<bool>
{
    public SwitchParameter(string name, string description, string[] flags, string[] args, bool defaultValue = false) : base(name, description, flags)
    {
        Value = args.Intersect(Flags).Any() ? !defaultValue : defaultValue;
        HasValue = true;
    }
}