namespace DragonFruit2.SclWrappers;

public class Argument<TValue> : System.CommandLine.Argument<TValue>, IHasDataDefinition
{
    public Argument(ArgumentDataDefinition<TValue> argumentDefinition)
        : base(argumentDefinition.PosixName)
    {
        ArgumentDefinition = argumentDefinition;
    }

    public ArgumentDataDefinition<TValue> ArgumentDefinition { get; }

    public DataDefinition DataDefinition => ArgumentDefinition;

}
