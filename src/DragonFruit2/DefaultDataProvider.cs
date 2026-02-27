using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public class DefaultDataProvider<TRootArgs> : DataProvider<TRootArgs>
     where TRootArgs : ArgsRootBase<TRootArgs>
{
    public DefaultDataProvider(Builder<TRootArgs> builder)
        : base(builder)
    {
    }


    protected override bool TryGetValue<TValue>(MemberDataDefinition<TValue> memberDefinition,
                                             Result<TRootArgs> result,
                                             [NotNullWhen(true)] out TValue value)
    {

        if (result.DataValues is DataValues<TRootArgs> dataValues)
        {
            if (memberDefinition.TrySetDefault(dataValues, out var outValue))
            {
                value = outValue;
                return true;
            }
        }
        value = default!;
        return false;
    }
}
