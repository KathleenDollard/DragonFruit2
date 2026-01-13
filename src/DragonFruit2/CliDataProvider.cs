using System.CommandLine;

namespace DragonFruit2;

public class CliDataProvider<TRootArgs> : DataProvider
            where TRootArgs : IArgs<TRootArgs>
{

    public string[] InputArgs
    {
        get;
        private set;

    }

    internal void SetInputArgs(string[] args)
    {
        {
            InputArgs = args;
            ParseResult = RootCommand?.Parse(InputArgs);
            ArgsBuilderCache<TRootArgs>.ActiveArgsBuilder = null;
            var x = ParseResult?.Invoke();
        }
    }

    public Command? RootCommand
    {
        get;
        set;
        //{
        //    field = value;
        //    field?.SetAction(parseResult =>
        //    {
        //        var parseResult = RootCommand?.Parse(InputArgs);
        //        parseResult.Invoke();
        //        return 0;
        //    });
        //}
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
