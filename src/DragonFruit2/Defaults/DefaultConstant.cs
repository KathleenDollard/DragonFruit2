using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public class DefaultConstant<TValue> : DefaultDefinition<TValue>
{
    private TValue _defaultValue;

    public static DefaultConstant<TValue> Create(string valueName, TValue  value)
    { return new DefaultConstant<TValue>(valueName, value); }

    public DefaultConstant(string valueName, TValue value)
        : base(valueName, $"value")
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
    public override bool TrySetDefaultValue(DataValues dataValues, [NotNullWhen(true)] out TValue value)
    {
        value = _defaultValue!;
        return true;
    }
}

// TODO: We need analyzers to check the type of the default value matches the property type, and that the attribute constructor parameter appears as a property on the attribute
[DefaultAttributeInfo(typeof(DefaultConstant<>))]
public sealed class DefaultAttribute
{

    public DefaultAttribute(object defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public object DefaultValue { get; }
}

public static class DefaultConstantExtensions
{
    extension<TValue>(MemberDataDefinition<TValue> memberDefinition)
    {
        public void Default(TValue defaultValue)
        {
            throw new NotImplementedException();
        }
    }
}
