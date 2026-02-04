using System.CommandLine;
using System.Xml.Linq;

namespace DragonFruit2;


public abstract class CommandDataDefinition : DataDefinition
{
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

    private readonly List<OptionDataDefinition> _options = [];
    private readonly List<ArgumentDataDefinition> _arguments = [];
    private readonly List<CommandDataDefinition> _subcommands = [];

    public IEnumerable<OptionDataDefinition> Options => _options;
    public IEnumerable<ArgumentDataDefinition> Arguments => _arguments;
    public IEnumerable<CommandDataDefinition> Subcommands => _subcommands;

    public void Add(OptionDataDefinition option) => _options.Add(option);
    public void Add(ArgumentDataDefinition argument) => _arguments.Add(argument);
    public void Add(CommandDataDefinition subcommand) => _subcommands.Add(subcommand);

    internal void InitializeMember(OptionDataDefinition optionDefinition, Func<string, Option> makeOption)
    {
        throw new NotImplementedException();
    }
}

public class CommandDataDefinition<TRootArgs> : CommandDataDefinition
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
