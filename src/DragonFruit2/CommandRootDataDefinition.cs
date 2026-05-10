using DragonFruit2.Defaults;
using DragonFruit2.Validators;

namespace DragonFruit2;


public abstract class CommandRootDataDefinition<TRootCommand> : CommandDataDefinition<TRootCommand, TRootCommand>
{

    private readonly List<CommandDataDefinition> _subcommands = [];
    private readonly List<DefaultDefinition> _defaultDefinitions = [];
    private readonly Dictionary<string, Validator> _validators = [];

    public CommandRootDataDefinition(Type command,
                                 CommandDataDefinition? parentDataDefinition,
                                 CommandDataDefinition? rootDataDefinition)
        : base(parentDataDefinition, rootDataDefinition)
    { }


}

