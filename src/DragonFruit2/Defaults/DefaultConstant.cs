using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public class DefaultConstant<TValue> : DefaultDefinition<TValue>
{
    private TValue _defaultValue;

    public static DefaultConstant<TValue> Create(TValue value)
    { return new DefaultConstant<TValue>(value); }

    public DefaultConstant(TValue value)
        : base($"value")
    {
        _defaultValue = value;
    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// This method currently allows setting a default value to default.
    /// </remarks>
    /// <param name="dataValues"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override bool TryGetDefaultValue(DataValues dataValues, [NotNullWhen(true)] out TValue value)
    {
        value = _defaultValue!;
        return true;
    }
}
