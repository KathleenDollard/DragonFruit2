using System.CommandLine;

namespace DragonFruit2;


public abstract class CommandDataDefinition : DataDefinition
{
    public abstract IEnumerable<TReturn> CreateMembers<TReturn>(ICreatesMembers<TReturn> dataProvider);

    private readonly Dictionary<string, MemberDataDefinition> _members = [];
    private readonly List<CommandDataDefinition> _subcommands = [];

    public CommandDataDefinition(Type rootArgs, 
                                 CommandDataDefinition? parentDataDefinition,
                                 CommandDataDefinition? rootDataDefinition)
        : base(rootArgs.Name)
    {
        ParentDataDefinition = parentDataDefinition;
        RootDataDefinition = rootDataDefinition ?? this;
        ArgsType = rootArgs;
    }

    public MemberDataDefinition this[string memberName]
    {
        get => _members[memberName];
    }

    public Type ArgsType { get; }

    public CommandDataDefinition? ParentDataDefinition { get; }
    public CommandDataDefinition RootDataDefinition { get; }

    // IsOptionStyle is not yet implemented, and will indicate whether the option performs an action, thus behaving like a command
    public bool IsOptionStyle { get; set; }

    public IEnumerable<OptionDataDefinition> Options => _members.Values.OfType<OptionDataDefinition>();
    public IEnumerable<ArgumentDataDefinition> Arguments => _members.Values.OfType<ArgumentDataDefinition>();
    public IEnumerable<CommandDataDefinition> Subcommands => _subcommands;

    public void Add(OptionDataDefinition option) => _members.Add(option.Name, option);
    public void Add(ArgumentDataDefinition argument) => _members.Add(argument.Name, argument);
    public void Add(CommandDataDefinition subcommand) => _subcommands.Add(subcommand);

    internal void InitializeMember(OptionDataDefinition optionDefinition, Func<string, Option> makeOption)
    {
        throw new NotImplementedException();
    }
}

public abstract class CommandDataDefinition<TRootArgs> : CommandDataDefinition
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    private Func<DataValues<TRootArgs>> _getDataValues;
    public CommandDataDefinition(CommandDataDefinition? parentDataDefinition,
                                 CommandDataDefinition? rootDataDefinition,
                                 Func<DataValues<TRootArgs>> getDataValues)
        : base(typeof(TRootArgs), parentDataDefinition, rootDataDefinition)
    {
        _getDataValues = getDataValues;
    }

    internal DataValues<TRootArgs> CreateDataValues()
    {
        return _getDataValues();
    }
}
