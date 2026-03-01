using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public class DefaultConstant<TValue> : DefaultDefinition<TValue>
{
    private TValue _defaultValue;

    public DefaultConstant(MemberDataDefinition<TValue> memberDefinition, TValue value)
        : base(memberDefinition, $"value")
    {
        _defaultValue = value;
    }

    /// <inheritdoc/>
    public override bool TryGetDefaultValue(DataValues dataValues, MemberDataDefinition<TValue> dataValue, [NotNullWhen(true)] out TValue defaultValue)
    {
        defaultValue = _defaultValue!;
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
