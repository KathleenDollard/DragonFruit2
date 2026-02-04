namespace DragonFruit2.SclWrappers;

public class Command : System.CommandLine.Command
{
    public Command(string name, ArgumentDataDefinition commandDefinition, string? description = null) 
        : base(name, description)
    {
        CommandDefinition = commandDefinition;
    }

    public ArgumentDataDefinition CommandDefinition { get; }
}
