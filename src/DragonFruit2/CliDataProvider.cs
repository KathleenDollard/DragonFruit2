using DragonFruit2.Validators;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace DragonFruit2;

public class CliDataProvider<TRootArgs> : DataProvider, IActiveArgsBuilderProvider<TRootArgs>
            where TRootArgs : class, IArgs<TRootArgs>
{
    public CliDataProvider(Builder<TRootArgs> builder)
    {
        InputArgs = builder.CommandLineArguments;
    }

    public string[]? InputArgs { get; }

    public (IEnumerable<ValidationFailure>? failures, ArgsBuilder<TRootArgs>? builder) GetActiveArgsBuilder()
    {
        ParseResult = RootCommand?.Parse(InputArgs);
        ArgsBuilderCache<TRootArgs>.ActiveArgsBuilder = null;
        var _ = ParseResult?.Invoke();
        var failures = ParseResult is not null && ParseResult.Errors.Any()
                        ? TransformErrors(ParseResult.Errors)
                        : Enumerable.Empty<ValidationFailure>();

        return (failures, ArgsBuilderCache<TRootArgs>.GetActiveArgsBuilder());
    }

    private IEnumerable<ValidationFailure>? TransformErrors(IReadOnlyList<ParseError> errors)
    {
        return errors.Select(CreateValidationFailure);

        static ValidationFailure CreateValidationFailure(ParseError error)
            => new(ValidationId.SystemCommandLine.ToValidationIdString(),
                   error.Message,
                   string.Empty,
                   DiagnosticSeverity.Error);
    }


    public Command? RootCommand
    {
        get;
        set;
    }

    public ParseResult? ParseResult
    {
        get => field ??= RootCommand?.Parse(InputArgs);
        private set;
    }

    public Dictionary<string, Symbol> LookupByName { get; set; } = [];

    public override bool TryGetValue<TValue>(string key, object[] alternateKeys, out DataValue<TValue> trialValue)
    {
        if (RootCommand is null) throw new ArgumentNullException(nameof(RootCommand));
        ParseResult ??= RootCommand.Parse(InputArgs);

        var symbol = LookupByName[key];
        if (symbol is not null)
        {
            var symbolResult = ParseResult.GetResult(symbol);
            if (symbolResult is not null && symbolResult.Tokens.Count > 0)
            {
                // Value was provided. 
                // TODO: Test bool flags
                trialValue = GetValue(ParseResult, symbol);
                return true;
            }
        }
        trialValue = DataValue<TValue>.CreateEmpty();
        return false;

        DataValue<TValue> GetValue(ParseResult parseResult, Symbol symbol)
        {
            TValue? value = symbol switch
            {
                Argument argument => parseResult.GetValue<TValue>(argument.Name),
                Option option => parseResult.GetValue<TValue>(option.Name),
                _ => throw new InvalidOperationException("Unsupported symbol type")
            };

            return value is null
                ? DataValue<TValue>.CreateEmpty()
                : DataValue<TValue>.Create(value!, this);
        }
    }

    public void AddNameLookup(string name, Symbol symbol)
    {
        LookupByName[name] = symbol;
    }

}
