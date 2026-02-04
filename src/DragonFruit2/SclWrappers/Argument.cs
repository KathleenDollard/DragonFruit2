namespace DragonFruit2.SclWrappers;

public class Argument<T> : System.CommandLine.Argument<T>
{
    public Argument(string name, ArgumentDataDefinition argumentDefinition)
        : base(name)
    {
        ArgumentDefinition = argumentDefinition;
    }

    public ArgumentDataDefinition ArgumentDefinition { get; }
}
