namespace DragonFruit2;


public class CommandDataDefinition : DataDefinition
{
    public CommandDataDefinition(Type argsType,
                                 CommandDataDefinition? parentDataDefinition,
                                 CommandDataDefinition? rootDataDefinition)
        : base(argsType, argsType.Name)
    {
        ParentDataDefinition = parentDataDefinition;
        RootDataDefinition = rootDataDefinition ?? this;
    }
    public CommandDataDefinition? ParentDataDefinition { get;  }
    public CommandDataDefinition RootDataDefinition { get;  }

    // IsOptionStyle is not yet implemented, and will indicate whether the option performs an action, thus behaving like a command
    public bool IsOptionStyle { get; set; }

    private readonly List<OptionDataDefinition> _options = [];
    private readonly List<ArgumentDataDefinition> _arguments = [];
    private readonly List<CommandDataDefinition> _subcommands = [];

    public IEnumerable<OptionDataDefinition> Options => _options;
    public IEnumerable<ArgumentDataDefinition> Arguments => _arguments;
    public IEnumerable<CommandDataDefinition> Subcommands => _subcommands;

    public void Add(OptionDataDefinition option) => _options.Add(option);
    public void Add(ArgumentDataDefinition argument) => _arguments.Add(argument);
    public void Add(CommandDataDefinition subcommand) => _subcommands.Add(subcommand);
}

public class CommandDataDefinition<TArgs> : CommandDataDefinition
{
    public CommandDataDefinition(CommandDataDefinition? parentDataDefinition, CommandDataDefinition? rootDataDefinition) 
        : base(typeof(TArgs),parentDataDefinition, rootDataDefinition)
    {
    }
}
