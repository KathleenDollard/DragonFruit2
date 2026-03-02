using DragonFruit2.Validators;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public class DefaultConstant<TValue> : DefaultDefinition<TValue>
{
    private TValue _defaultValue;

    public DefaultConstant(MemberDataDefinition<TValue> memberDefinition, TValue defaultValue)
    {
        _defaultValue = defaultValue;
    }

    public override string Description => $"{_defaultValue}";


    /// <inheritdoc/>
    public override bool TryGetDefaultValue(DataValues dataValues, MemberDataDefinition<TValue> memberDefinition, [NotNullWhen(true)] out TValue defaultValue)
    {
        defaultValue = _defaultValue!;
        return true;
    }
}

// TODO: We need analyzers to check the type of the default value matches the property type, and that the attribute constructor parameter appears as a property on the attribute
[DefaultAttribute(typeof(DefaultConstant<>))]
public sealed class DefaultConstantAttribute : DefaultBaseAttribute
{

    public DefaultConstantAttribute(object defaultValue)
        : base(typeof(DefaultConstant<>))
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
