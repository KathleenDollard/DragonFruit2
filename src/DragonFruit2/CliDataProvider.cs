using System.CommandLine;

namespace DragonFruit2;

public class CliDataProvider<TArgs> : DataProvider
            where TArgs : IArgs<TArgs>
{
    //public CliDataProvider(string[] inputArgs)
    //{
    //    InputArgs = inputArgs ?? System.Environment.GetCommandLineArgs();
    //}

    public string[] InputArgs
    {
        get;
        set
        {
            field = value;
            ParseResult = null;
        }
    }
    public Command? RootCommand
    {
        get;
        set
        {
            field = value;
            field?.SetAction(parseResult =>
            {
                return 0;
            });
        }
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
        //// TODO: The invocation should be replaced with direct calls to error reporting if possible
        //var returnCode = parseResult.Invoke();
        //if (returnCode != 0)
        //{
        //    trialValue = DataValue<TValue>.CreateEmpty();
        //    return false;
        //}

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
