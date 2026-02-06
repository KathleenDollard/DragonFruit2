using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public class DefaultConstant<TValue> : DefaultDefinition<TValue>
{
    private TValue _value;

    public static DefaultConstant<TValue> Create(TValue value)
    { return new DefaultConstant<TValue>(value); }

    public DefaultConstant(TValue value)
        : base($"value")
    {
        _value = value;
    }

    public override bool TryGetDefaultValue(DataValues dataValues, [NotNullWhen(true)] out TValue value)
    {
        if (EqualityComparer<TValue>.Default.Equals(_value, default))
        {
            value = default!;
            return true;
        }
        value = _value!;
        return true;
    }
}
