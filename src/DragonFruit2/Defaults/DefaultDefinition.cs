using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public abstract class DefaultDefinition
{ }

public abstract class DefaultDefinition<TValue> : DefaultDefinition
{
    private readonly string[] _dependentValueNames;

    public DefaultDefinition(string description, params string[] dependentValueNames)
    {
        Description = description;
        _dependentValueNames = dependentValueNames;
    }
    public abstract bool TryGetDefaultValue(DataValues dataValues, [NotNullWhen(true)] out TValue value);
    public string Description { get; }
}

//public class DefaultCalculated<TValue> : DefaultDefinition<TValue>
//{
//    private Func<TValue> _calculator;

//    public DefaultCalculated(Func<TValue> calculator, string description)
//       : base(description)
//    {
//        _calculator = calculator;
//    }
//    public override bool GetDefault(DataValues dataValues)
//    {
//        return _calculator();
//    }
//}

