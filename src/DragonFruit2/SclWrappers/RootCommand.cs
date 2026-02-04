namespace DragonFruit2.SclWrappers;

public class RootCommand : System.CommandLine.RootCommand, IHasDataDefinition
{
    public RootCommand(CommandDataDefinition commandDefinition)
        : base(string.Empty)
    {
        CommandDefinition = commandDefinition;
    }

    public CommandDataDefinition CommandDefinition { get; }

    public DataDefinition DataDefinition => CommandDefinition;
}
