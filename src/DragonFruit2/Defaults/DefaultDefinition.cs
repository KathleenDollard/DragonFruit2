using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public abstract class DefaultDefinition
{
    protected abstract MemberDataDefinition GetMemberDefinition();
    public MemberDataDefinition MemberDataDefinition => GetMemberDefinition();
}

public abstract class DefaultDefinition<TValue> : DefaultDefinition
{
    private readonly string[] _dependentValueNames;

    public DefaultDefinition(MemberDataDefinition<TValue> memberDefinition, string description, params string[] dependentValueNames)
    {
        MemberDataDefinition = memberDefinition;
        Description = description;
        _dependentValueNames = dependentValueNames;
    }


    public string Description { get; }

    public new MemberDataDefinition<TValue> MemberDataDefinition { get; }
    protected override MemberDataDefinition GetMemberDefinition() => MemberDataDefinition;

    // The MemberDataDefinition is passed here to allow data inference to work. DataValue<TValue> is not used 
    // becaues it is very important to the default system that the DataValue is not set in the Default provider,
    // but rather the value is returned.
    public abstract bool TryGetDefaultValue(DataValues dataValues,
                                                    MemberDataDefinition<TValue> memberDefinition,
                                                    [NotNullWhen(true)] out TValue value);

    //public override bool TryGetDefaultValue<TValueLocal>(DataValues dataValues,
    //                                            DataValue<TValueLocal> dataValue,
    //                                            [NotNullWhen(true)] out TValueLocal value)
    //{
    //    if (dataValue is not DataValue<TValue> typedMemberDefinition)
    //    {
    //        throw new InvalidOperationException($"TryGetValue called with wrong generic type. {typeof(TValue)} was expected, and {typeof(TValueLocal)} was received.");
    //    }
    //    return TryGetDefaultValue(dataValues, MemberDataDefinition, out (TValueLocal)value!);
    //}

    //public abstract bool TryGetDefaultValue(DataValues dataValues,
    //                                                MemberDataDefinition<TValue> dataValue,
    //                                                [NotNullWhen(true)] out TValue value);
}


