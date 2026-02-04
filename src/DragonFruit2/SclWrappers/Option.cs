namespace DragonFruit2.SclWrappers;

public class Option<T> : System.CommandLine.Option<T>, IHasDataDefinition
{
    public Option(OptionDataDefinition optionDefinition)
        : base($"--{optionDefinition.PosixName}")
    {
        OptionDefinition = optionDefinition;
    }

    public OptionDataDefinition OptionDefinition { get; }
    public DataDefinition DataDefinition => OptionDefinition;

}
