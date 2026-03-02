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

    public DefaultDefinition(params string[] dependentValueNames)
    {
        _dependentValueNames = dependentValueNames;
    }


    public abstract string Description { get; }

    protected override MemberDataDefinition GetMemberDefinition() => MemberDataDefinition;

    // The MemberDataDefinition is passed here to allow data inference to work. DataValue<TValue> is not used 
    // becaues it is very important to the default system that the DataValue is not set in the Default provider,
    // but rather the value is returned.
    public abstract bool TryGetDefaultValue(DataValues dataValues,
                                                    MemberDataDefinition<TValue> memberDefinition,
                                                    [NotNullWhen(true)] out TValue value);

}


