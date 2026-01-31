using DragonFruit2.Validators;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.ComponentModel;

namespace DragonFruit2;

public class CliDataProvider<TRootArgs> : DataProvider<TRootArgs>, IActiveArgsBuilderProvider<TRootArgs>
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    public CliDataProvider(Builder<TRootArgs> builder) 
        : base(builder)
    { }

    public string[]? InputArgs => Builder.CommandLineArguments;

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

    public Dictionary<(Type argsType, string propertyName), Symbol> LookupSymbol { get; set; } = [];

    public override bool TryGetValue<TValue>((Type argsType, string propertyName) key, DataValue<TValue> dataValue)
    {
        if (RootCommand is null) throw new ArgumentNullException(nameof(RootCommand));
        if (InputArgs is null) throw new ArgumentNullException(nameof(InputArgs));
        ParseResult ??= RootCommand.Parse(InputArgs);

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
            else if ((typeof(TValue) == typeof( bool) || typeof(TValue) == typeof( bool?)) 
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

}
