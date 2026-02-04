namespace DragonFruit2.SclWrappers;

public class Option<T> : System.CommandLine.Option<T>
{
    public Option(string name, OptionDataDefinition optionDefinition)
        : base(name)
    {
        OptionDefinition = optionDefinition;
    }

    public OptionDataDefinition OptionDefinition { get; }
}
