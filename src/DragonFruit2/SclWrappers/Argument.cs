namespace DragonFruit2.SclWrappers;

public class Argument<T> : System.CommandLine.Argument<T>, IHasDataDefinition
{
    public Argument(ArgumentDataDefinition argumentDefinition)
        : base(argumentDefinition.PosixName)
    {
        ArgumentDefinition = argumentDefinition;
    }

    public ArgumentDataDefinition ArgumentDefinition { get; }

    public DataDefinition DataDefinition => ArgumentDefinition;

}
