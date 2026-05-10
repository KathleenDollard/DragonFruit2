using DragonFruit2.Defaults;
using DragonFruit2.Validators;

namespace DragonFruit2;


public abstract class CommandDataDefinition : DataDefinition
{
    protected abstract MemberDataDefinition? GetMemberDefinition(string memberName);
    public abstract IEnumerable<TReturn> Operate<TReturn>(IOperationOnMemberDefinition<TReturn> operationContainer);

    private readonly List<CommandDataDefinition> _subcommands = [];
    private readonly List< DefaultDefinition> _defaultDefinitions = [];
    private readonly Dictionary<string, Validator> _validators = [];

    public CommandDataDefinition(Type command,
                                 CommandDataDefinition? parentDataDefinition,
                                 CommandDataDefinition? rootDataDefinition)
        : base(command.Name.EndsWith("Command") ? command.Name.Substring(0, command.Name.Length - 7) : command.Name)
    {
        ParentDataDefinition = parentDataDefinition;
        RootDataDefinition = rootDataDefinition ?? this;
        CommandType = command;
    }


    public Type CommandType { get; }
    public CommandDataDefinition? ParentDataDefinition { get; }
    public CommandDataDefinition RootDataDefinition { get; }

    // IsOptionStyle is not yet implemented, and will indicate whether the option performs an action, thus behaving like a command
    public bool IsOptionStyle { get; set; }
    public IEnumerable<DefaultDefinition> DefaultDefinitions => _defaultDefinitions;
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

    internal void RegisterDefault<TValue>(MemberDataDefinition<TValue> memberDataDefinition, DefaultDefinition<TValue> defaultDefinition)
    {
        _defaultDefinitions.Add( defaultDefinition);
    }

}

public abstract class CommandDataDefinition<TRootCommand> : CommandDataDefinition
{
    protected CommandDataDefinition(Type commandType,
                                    CommandDataDefinition? parentDataDefinition,
                                    CommandDataDefinition? rootDataDefinition)
        : base(commandType, parentDataDefinition, rootDataDefinition)
    {    }

    public Func<DataValues<TRootCommand>>? GetDataValues { get; protected set; }

    internal DataValues<TRootCommand> CreateDataValues()
    {
        if (GetDataValues is null)
        {
            throw new InvalidOperationException($"Generation may not have run, try rebuilding. GetDataValues is not set.");
        }
        return GetDataValues();
    }

}

public abstract class CommandDataDefinition<TCommand, TRootCommand> : CommandDataDefinition<TRootCommand>
    where TCommand : TRootCommand
{
    public CommandDataDefinition(CommandDataDefinition? parentDataDefinition,
                                 CommandDataDefinition? rootDataDefinition)
        : base(typeof(TCommand), parentDataDefinition, rootDataDefinition)
    { }

}
