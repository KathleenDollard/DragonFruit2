namespace DragonFruit2.SclWrappers;

public class Argument<TValue> : System.CommandLine.Argument<TValue>, IHasDataDefinition
{
    public Argument(ArgumentDataDefinition<TValue> argumentDefinition)
        : base(argumentDefinition.PosixName)
    {
        ArgumentDataDefinition = argumentDefinition;
    }

    public ArgumentDataDefinition<TValue> ArgumentDataDefinition { get; }

    public DataDefinition DataDefinition => ArgumentDataDefinition;

}
