namespace DragonFruit2.SclWrappers;

public class Option<TValue> : System.CommandLine.Option<TValue>, IHasDataDefinition
{
    public Option(OptionDataDefinition<TValue> optionDefinition)
        : base($"--{optionDefinition.PosixName}")
    {
        OptionDefinition = optionDefinition;
        Recursive = optionDefinition.Recursive;
    }

    public OptionDataDefinition<TValue> OptionDefinition { get; }
    public DataDefinition DataDefinition => OptionDefinition;

}
