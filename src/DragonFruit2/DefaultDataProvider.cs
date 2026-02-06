using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public class DefaultDataProvider<TRootArgs> : DataProvider<TRootArgs>
     where TRootArgs : ArgsRootBase<TRootArgs>

{
    public DefaultDataProvider(Builder<TRootArgs> builder)
        : base(builder)
    {
    }

    private readonly Dictionary<(Type argsType, string propertyName), object> defaultValues = new();

    public override bool TryGetValue<TValue>(MemberDataDefinition<TValue> memberDefinition,
                                             Result<TRootArgs> result,
                                             [NotNullWhen(true)] out TValue value)
    {
        if (result.DataValues is DataValues<TRootArgs> dataValues)
        {
            if (memberDefinition.TryGetDefault(dataValues, out var outValue))
            {
                value = outValue;
                return true;
            }
        }
        value = default!;
        return false;
    }

    public void RegisterDefault<TValue>(Type argsType, string propertyName, TValue value)
    {
        if (value is not null && !value.Equals(default(TValue)))
        {
            defaultValues[(argsType, propertyName)] = value;
        }
    }
}
