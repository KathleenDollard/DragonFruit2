using DragonFruit2.Validators;
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
    public override bool TryGetDefaultValue(DataValues dataValues, [NotNullWhen(true)] out TValue value)
    {
        value = _defaultValue!;
        return true;
    }
}

[DefaultAttributeInfo(typeof(DefaultConstant<>))]
public sealed class DefaultAttribute : DefaultBaseAttribute
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
            memberDefinition.RegisterValidator(new DefaultConstant<TValue>(memberDefinition.DefinitionName, compareWithValue));
        }
    }
}
