using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public class DefaultDataProvider<TRootArgs> : DataProvider<TRootArgs>
     where TRootArgs : ArgsRootBase<TRootArgs>
{
    //// TODO: This a temporary hack to get the default system working
    //private static readonly DefaultDataProvider<TRootArgs> _instance = new(null);
    //public static DefaultDataProvider<TRootArgs> Instance() => _instance;
    public DefaultDataProvider(Builder<TRootArgs> builder)
        : base(builder)
    {
    }


    protected override bool TryGetValue<TValue>(MemberDataDefinition<TValue> memberDefinition,
                                             Result<TRootArgs> result,
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
