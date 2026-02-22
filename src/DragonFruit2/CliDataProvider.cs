using DragonFruit2.Validators;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace DragonFruit2;

public class CliDataProvider<TRootArgs> : DataProvider<TRootArgs>, IActiveArgsProvider<TRootArgs>
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    public CliDataProvider(Builder<TRootArgs> builder)
        : base(builder)
    { }

    private IEnumerable<Diagnostic>? TransformErrors(IReadOnlyList<ParseError> errors)
    {
        return errors.Select(CreateValidationFailure);

        static Diagnostic CreateValidationFailure(ParseError error)
            => new(DiagnosticId.SystemCommandLine.ToValidationIdString(),
                   DiagnosticSeverity.Error,
                   string.Empty,
                   error.Message);
    }
    private int cachedRunId;
    public Command? RootCommand { get; set; }
    /// <summary>
    /// This is the System.CommandLine.ParseResult that is used internally.
    /// </summary>
    /// <remarks>
    /// A single value is cached, which is used during a single run and if there is some reason another
    /// call is made with the same command line args, sucn as would happen if we support scripts
    /// <br/>
    /// Note that this cache is via this class, which is hte TRootArgs generic, if there are multiple root args, 
    /// such as might happen in an interactive or scripting scenario, they will be separately cached.
    /// </remarks>
    public ParseResult? ParseResult { get; private set; }

    private void InitializeRun(Result<TRootArgs> result)
    {
        if (RootCommand is null) throw new InvalidOperationException("RootCommand cannot be null");
        ParseResult = RootCommand.Parse(result.CommandLineArguments);
        cachedRunId = result.RunId;
    }

    public Dictionary<(Type argsType, string propertyName), Symbol> LookupSymbol { get; set; } = [];

    public override void Initialize(Builder<TRootArgs> builder, CommandDataDefinition<TRootArgs> commandDefinition, Result<TRootArgs> result)
    {
        RootCommand = new SclWrappers.RootCommand(commandDefinition);
        InitializeCommand(RootCommand, builder, commandDefinition, result);
    }

    private void InitializeCommand(System.CommandLine.Command command, Builder<TRootArgs> builder, CommandDataDefinition commandDefinition, Result<TRootArgs> result)
    {
        var memberSymbols = commandDefinition.Operate(new CreateFromMembersOperation(this));
        command.AddRange(memberSymbols);

        foreach (var subcommandDefinition in commandDefinition.Subcommands)
        {
            var subCommand = new SclWrappers.Command(subcommandDefinition);
            InitializeCommand(subCommand, builder, subcommandDefinition, result);
            command.Add(subCommand);
        }
        command.SetAction(p => ParseResult = p);
    }

    public struct CreateFromMembersOperation : IOperationOnMemberDefinition<Symbol>
    {
        private CliDataProvider<TRootArgs> _dataProvider;
        public CreateFromMembersOperation(CliDataProvider<TRootArgs> dataProvider)
        {
            _dataProvider = dataProvider;
        }
        public Symbol Operate<TValue>(MemberDataDefinition<TValue> memberDefinition)
        {
            Symbol newMember = memberDefinition switch
            {
                OptionDataDefinition<TValue> optionDefinition => new SclWrappers.Option<TValue>(optionDefinition),
                ArgumentDataDefinition<TValue> argumentDefinition => new SclWrappers.Argument<TValue>(argumentDefinition),
                _ => throw new InvalidOperationException("Unsupported member definition type")
            };
            _dataProvider.AddNameLookup((memberDefinition.CommandDefinition.ArgsType, memberDefinition.DefinitionName), newMember);
            return newMember;
        }
    }

    public override bool TryGetValue<TValue>(MemberDataDefinition<TValue> memberDefinition, Result<TRootArgs> result, out TValue value)
    {
        if (ParseResult is null || cachedRunId != result.RunId)
        {
            InitializeRun(result);
        }
        if (ParseResult is null)
        {
            value = default!;
            return false;
        }
        var key = (memberDefinition.CommandDefinition.ArgsType, memberDefinition.DefinitionName);
        var symbol = LookupSymbol[key];
        if (symbol is not null)
        {
            var symbolResult = ParseResult.GetResult(symbol);
            if (symbolResult is not null)
            {
                // Symbol was found, value may or may not have been provided
                if (TryGetParsedValue(ParseResult, symbol, out value))
                {
                    return true;
                }
            }
        }
        value = default!;
        return false;



        static bool TryGetParsedValue(ParseResult parseResult, Symbol symbol, [NotNullWhen(true)] out TValue outValue)
        {
            var symbolResult = parseResult.GetResult(symbol);
            if (symbolResult is null)
            {
                outValue = default!;
                return false;
            }

            if (symbolResult.Tokens.Any())
            {
                // Except for Boolean switches, do not use default values, but only those specified via tokens
                TValue? value = symbol switch
                {
                    Argument argument => parseResult.GetValue<TValue>(argument.Name),
                    Option option => parseResult.GetValue<TValue>(option.Name),
                    _ => throw new InvalidOperationException("Unsupported symbol type")
                };
                if (value is not null)
                {
                    outValue = value;
                    return true;
                }
            }
            else if ((typeof(TValue) == typeof(bool) || typeof(TValue) == typeof(bool?))
                   && symbolResult is OptionResult optionResult
                   && optionResult.Option.ValueType == typeof(bool))
            {
                if (optionResult.IdentifierToken is null)
                {
                    outValue = default!;
                    return false;
                }
                ;
                bool value = true; // If there were tokens, or there was no identifier token, it was already handled
                if (value is TValue castValue) // This is to silence NRT warning
                {
                    outValue = castValue;
                    return true;
                }
                // We should not be able to get here sinde this else block checks the TValue type. 
                // We might be able to clean up the messy generics here.
            }
            outValue = default!;
            return false;
        }
    }

    private void AddNameLookup((Type argsType, string propertyName) key, Symbol symbol)
    {
        LookupSymbol[key] = symbol;
    }

    public bool TryGetActiveArgsDefinition(Result<TRootArgs> result, [NotNullWhen(true)] out CommandDataDefinition<TRootArgs> activeCommandDefinition)
    {

        if (ParseResult is null || cachedRunId != result.RunId)
        {
            InitializeRun(result);
        }
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

    public class CliDataProviderInfo
    {
        public ParseResult ParseResult { get; }

        public CliDataProviderInfo(ParseResult parseResult)
        {
            ParseResult = parseResult;
        }


        //private ParseResult? _parseResult = null;

        //public string[]? InputArgs => Builder.CommandLineArguments;

        ///// <summary>
        ///// This is the System.CommandLine.ParseResult that is used internally.
        ///// </summary>
        ///// <remarks>
        ///// A single value is cached, which is used during a single run and if there is some reason another
        ///// call is made with the same command line args.
        ///// <br/>
        ///// Note that this cache is via the generic, if there are multiple root args, such as in an 
        ///// interactive or scripting scenario, they will be separately cached.
        ///// </remarks>
        //public ParseResult? ParseResult
        //{
        //    get
        //    {
        //        if (RootCommand is null) throw new InvalidOperationException("RootCommand cannot be null");
        //        if (InputArgs is null) throw new InvalidOperationException("InputArgs cannot be null");
        //        if (field is null || _parseResultArgs != InputArgs)
        //        {
        //            field = RootCommand?.Parse(InputArgs);
        //            _parseResultArgs = InputArgs ?? [];
        //        }
        //        return field;
        //    }

        //    private set;
        //}



    }
}
