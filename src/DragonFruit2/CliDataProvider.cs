using DragonFruit2.Common;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace DragonFruit2;

public class CliDataProvider() : DataProvider
{
    public ParseResult? ParseResult { get; set; }
    public Dictionary<string, Symbol> LookupByName { get; set; } = [];

    public override bool TryGetValue<TValue>(string key, object[] alternateKeys, out DataValue<TValue> trialValue)
    {
        if (ParseResult is null) throw new InvalidOperationException("ParseResult is not set on CliDataProvider");

        if (alternateKeys.FirstOrDefault() is Symbol symbol)
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

        //SymbolResult? symbolResult;
        //try
        //{
        //    symbolResult = ParseResult.GetResult(searchFor);
        //}
        //catch
        //{
        //    symbolResult = ParseResult.GetResult($"--{searchFor}");
        //}

        //throw new NotImplementedException();

    }

    public void AddNameLookup(string name, Symbol symbol)
    {
        LookupByName[name] = symbol;
    }
}
