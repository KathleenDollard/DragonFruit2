using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public class DefaultDataProvider<TRootCommand> : DataProvider<TRootCommand>
{
    //// TODO: This a temporary hack to get the default system working
    //private static readonly DefaultDataProvider<TRootCommand> _instance = new(null);
    //public static DefaultDataProvider<TRootCommand> Instance() => _instance;
    public DefaultDataProvider(Builder<TRootCommand> builder)
        : base(builder)
    {
    }


    protected override bool TryGetValue<TValue>(MemberDataDefinition<TValue> memberDefinition,
                                             Result<TRootCommand> result,
                                             [NotNullWhen(true)] out TValue value)
    {
        if (result.DataValues is null)
        {
            throw new InvalidOperationException("Unexpected null value for Result.DataValues");
        }

        var defaults = memberDefinition.Defaults;
        foreach (var defaultDefinition in defaults)
        {
            if (defaultDefinition.TryGetDefaultValue(result.DataValues, memberDefinition, out value))
            { return true; }
        }

        value = default!;
        return false;
    }
}
