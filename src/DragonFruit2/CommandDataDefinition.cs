using DragonFruit2.Defaults;
using DragonFruit2.Validators;

namespace DragonFruit2;


public abstract class CommandDataDefinition : DataDefinition
{
    protected abstract MemberDataDefinition? GetMemberDefinition(string memberName);
    public abstract IEnumerable<TReturn> Operate<TReturn>(IOperationOnMemberDefinition<TReturn> operationContainer);

    private readonly List<CommandDataDefinition> _subcommands = [];
    private readonly Dictionary<string, DefaultDefinition> _defaultDefinitions = [];
    private readonly Dictionary<string, Validator> _validators = [];

    public CommandDataDefinition(Type rootArgs,
                                 CommandDataDefinition? parentDataDefinition,
                                 CommandDataDefinition? rootDataDefinition)
        : base(rootArgs.Name)
    {
        ParentDataDefinition = parentDataDefinition;
        RootDataDefinition = rootDataDefinition ?? this;
        ArgsType = rootArgs;
    }


    public Type ArgsType { get; }

    public CommandDataDefinition? ParentDataDefinition { get; }
    public CommandDataDefinition RootDataDefinition { get; }

    // IsOptionStyle is not yet implemented, and will indicate whether the option performs an action, thus behaving like a command
    public bool IsOptionStyle { get; set; }

    //public IEnumerable<MemberDataDefinition> Members => _members.Values;
    public IEnumerable<CommandDataDefinition> Subcommands => _subcommands;

    public void Add(CommandDataDefinition subcommand) => _subcommands.Add(subcommand);

    public virtual void RegisterCustomizations()
    {
        // no op
    }

    public void RegisterValidator(string name, Validator validator)
    {

        _validators.Add(name, validator);
    }
}

public abstract class CommandDataDefinition<TRootArgs> : CommandDataDefinition
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    public CommandDataDefinition(CommandDataDefinition? parentDataDefinition,
                                 CommandDataDefinition? rootDataDefinition)
        : base(typeof(TRootArgs), parentDataDefinition, rootDataDefinition)
    {    }

    public Func<DataValues<TRootArgs>>? GetDataValues { get; protected set; }

    internal DataValues<TRootArgs> CreateDataValues()
    {
        if (GetDataValues is null)
        {
            throw new InvalidOperationException($"Generation may not have run, try rebuilding. GetDataValues is not set.");
        }
        return GetDataValues();
    }

}
