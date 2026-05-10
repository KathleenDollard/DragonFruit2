using DragonFruit2.Defaults;
using DragonFruit2.Validators;

namespace DragonFruit2;


public abstract class CommandRootDataDefinition<TRootCommand> : CommandDataDefinition<TRootCommand, TRootCommand>
{
    private Dictionary<CommandClass, CommandDataDefinition> commandDefinitionLookup = [];

    public CommandRootDataDefinition(Type command,
                                 CommandDataDefinition? parentDataDefinition,
                                 CommandDataDefinition? rootDataDefinition)
        : base(parentDataDefinition, rootDataDefinition)
    { }


}

