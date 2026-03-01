using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public class DefaultDataProvider<TRootArgs> : DataProvider<TRootArgs>
     where TRootArgs : ArgsRootBase<TRootArgs>
{
    // TODO: This a temporary hack to get the default system working
    private static readonly DefaultDataProvider<TRootArgs> _instance = new(null);
    public static DefaultDataProvider<TRootArgs> Instance() => _instance;
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

    public void SetDefaults()
    {
        while (true)
        {
            var anythingChanged = false;
            foreach (var defaultDefinition in CommandDefinition.DefaultDefinitions)
            {
                if (_values.TryGetValue(defaultDefinition.MemberDataDefinition, out var matchingDataValue))
                {
                    if (matchingDataValue.IsSet)
                    { continue; }
                }

                if (matchingDataValue.IsSet)
                { break; }

                // The following will fail if the dependent values are not present
                if (matchingDataValue.TrySetDefaultValue<TRootArgs>(defaultDefinition, this))
                {
                    anythingChanged = true;
                    break;
                }
            }
            if (!anythingChanged)
            {
                break;
            }
        }
    }
}
