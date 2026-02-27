namespace DragonFruit2.SclWrappers;

public class Option<TValue> : System.CommandLine.Option<TValue>, IHasDataDefinition
{
    public Option(OptionDataDefinition<TValue> optionDefinition)
        : base($"--{optionDefinition.PosixName}")
    {
        OptionDataDefinition = optionDefinition;
        Recursive = optionDefinition.Recursive;
    }

    public OptionDataDefinition<TValue> OptionDataDefinition { get; }
    public DataDefinition DataDefinition => OptionDataDefinition;

}
