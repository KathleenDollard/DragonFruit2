using DragonFruit2.Validators;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public class CliDataProvider<TRootArgs> : DataProvider<TRootArgs>, IActiveArgsProvider<TRootArgs>, ICreatesMembers<Symbol>
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    public CliDataProvider(Builder<TRootArgs> builder)
        : base(builder)
    { }

    // This is not a great way to cache the _parseResut if this method is reentrant
    private ParseResult? _parseResult = null;

    public string[]? InputArgs => Builder.CommandLineArguments;

    //public (IEnumerable<Diagnostic>? failures, ArgsBuilder<TRootArgs>? builder) GetActiveArgsBuilder()
    //{
    //    ParseResult = RootCommand?.Parse(InputArgs);
    //    ArgsBuilderCache<TRootArgs>.ActiveArgsBuilder = null;
    //    var _ = ParseResult?.Invoke();
    //    var failures = ParseResult is not null && ParseResult.Errors.Any()
    //                    ? TransformErrors(ParseResult.Errors)
    //                    : Enumerable.Empty<Diagnostic>();

    //    return (failures, ArgsBuilderCache<TRootArgs>.GetActiveArgsBuilder());
    //}

    private IEnumerable<Diagnostic>? TransformErrors(IReadOnlyList<ParseError> errors)
    {
        return errors.Select(CreateValidationFailure);

        static Diagnostic CreateValidationFailure(ParseError error)
            => new(DiagnosticId.SystemCommandLine.ToValidationIdString(),
                   error.Message,
                   string.Empty,
                   DiagnosticSeverity.Error);
    }

    public Command? RootCommand
    {
        get;
        set;
    }

    private string[] _parseResultArgs;

    /// <summary>
    /// This is the System.CommandLine.ParseResult that is used internally.
    /// </summary>
    /// <remarks>
    /// A single value is cached, which is used during a single run and if there is some reason another
    /// call is made with the same command line args.
    /// <br/>
    /// Note that this cache is via the generic, if there are multiple root args, such as in an 
    /// interactive or scripting scenario, they will be separately cached.
    /// </remarks>
    public ParseResult? ParseResult
    {
        get
        {
            if (RootCommand is null) throw new InvalidOperationException("RootCommand cannot be null");
            if (InputArgs is null) throw new InvalidOperationException("InputArgs cannot be null");
            if (field is null || _parseResultArgs != InputArgs)
            {
                field = RootCommand?.Parse(InputArgs);
                _parseResultArgs = InputArgs ?? [];
            }
            return field;
        }

        private set;
    }

    public Dictionary<(Type argsType, string propertyName), Symbol> LookupSymbol { get; set; } = [];

    public override void Initialize(Builder<TRootArgs> builder, CommandDataDefinition<TRootArgs> commandDefinition)
    {
        RootCommand = new SclWrappers.RootCommand(commandDefinition);
        InitializeCommand(RootCommand, builder, commandDefinition);
    }

    private void InitializeCommand(System.CommandLine.Command command, Builder<TRootArgs> builder, CommandDataDefinition commandDefinition)
    {
        var memberSymbols = commandDefinition.CreateMembers(this);
        command.AddRange(memberSymbols);

        foreach (var subcommandDefinition in commandDefinition.Subcommands)
        {
            var subCommand = new SclWrappers.Command(subcommandDefinition);
            InitializeCommand(subCommand, builder, subcommandDefinition);
            command.Add(subCommand);
        }

        command.SetAction(p => _parseResult = p);

    }

    public Symbol CreateMember<TValue>(CommandDataDefinition commandDefinition, string name)
    {
        var memberDefinition = commandDefinition[name];
        Symbol newMember = memberDefinition switch
        {
            OptionDataDefinition optionDefinition => new SclWrappers.Option<TValue>(optionDefinition),
            ArgumentDataDefinition argumentDefinition => new SclWrappers.Argument<TValue>(argumentDefinition),
            _ => throw new InvalidOperationException("Unsupported member definition type")
        };
        AddNameLookup((commandDefinition.ArgsType, name), newMember);
        return newMember;
    }

    public override bool TryGetValue<TValue>((Type argsType, string propertyName) key, DataValue<TValue> dataValue)
    {
        var symbol = LookupSymbol[key];
        if (symbol is not null)
        {
            var symbolResult = ParseResult.GetResult(symbol);
            if (symbolResult is not null)
            {
                // Symbol was found, value may or may not have been provided
                return SetDataValueIfProvided(ParseResult, symbol, this, dataValue);
            }
        }
        dataValue = null;
        return false;

        static bool SetDataValueIfProvided(ParseResult parseResult, Symbol symbol, DataProvider<TRootArgs> dataProvider, DataValue<TValue> dataValue)
        {
            var symbolResult = parseResult.GetResult(symbol);
            if (symbolResult is null) return false;

            var resultFound = false;

            if (symbolResult.Tokens.Any())
            {
                // Except for Boolean switches, do not use default values, but only those specified via tokens
                resultFound = true;
                TValue? value = symbol switch
                {
                    Argument argument => parseResult.GetValue<TValue>(argument.Name),
                    Option option => parseResult.GetValue<TValue>(option.Name),
                    _ => throw new InvalidOperationException("Unsupported symbol type")
                };
                if (value is not null)
                {
                    dataValue.SetValue(value, dataProvider);
                    return true;
                }
            }
            else if ((typeof(TValue) == typeof(bool) || typeof(TValue) == typeof(bool?))
                   && symbolResult is OptionResult optionResult
                   && optionResult.Option.ValueType == typeof(bool))
            {
                // If the user specified the option, use the default value, which is provided by GetValue
                if (optionResult.IdentifierTokenCount > 0)
                {
                    var defaultValueAsObject = optionResult.Option.GetDefaultValue();
                    // Had to cast to TValue to avoid compiler errors
                    if (defaultValueAsObject is bool && defaultValueAsObject is TValue defaultValue)
                    {
                        dataValue.SetValue(defaultValue, dataProvider);
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public void AddNameLookup((Type argsType, string propertyName) key, Symbol symbol)
    {
        LookupSymbol[key] = symbol;
    }

    public bool TryGetActiveArgsDefinition([NotNullWhen(true)] out CommandDataDefinition<TRootArgs> activeCommandDefinition)
    {

        if (ParseResult is null)
        {
            activeCommandDefinition = null!;
            return false;
        }
        var commandResult = ParseResult.CommandResult;
        var command = commandResult.Command;
        switch (command)
        {
            case SclWrappers.Command sclCommand and IHasDataDefinition { DataDefinition: CommandDataDefinition<TRootArgs> dataDefinition }:
                activeCommandDefinition = dataDefinition;
                return true;
            case SclWrappers.RootCommand sclRootCommand and IHasDataDefinition { DataDefinition: CommandDataDefinition<TRootArgs> dataDefinition }:
                activeCommandDefinition = dataDefinition;
                return true;
            default:
                activeCommandDefinition = null!;
                return false;
        }
        ;
    }


}
