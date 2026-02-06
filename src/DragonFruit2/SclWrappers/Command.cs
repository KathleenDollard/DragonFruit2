namespace DragonFruit2.SclWrappers;

public class Command : System.CommandLine.Command, IHasDataDefinition
{
    public Command(CommandDataDefinition commandDefinition)
        : base(commandDefinition.PosixName)
    {
        CommandDefinition = commandDefinition;
    }

    public CommandDataDefinition CommandDefinition { get; }

    public DataDefinition DataDefinition => CommandDefinition;

}
