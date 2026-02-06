namespace DragonFruit2;

public record class DataValue { }

public record class DataValue<TValue> : DataValue
{
    // TODO: Replace `argsType` with `key`
    public static DataValue<TValue> Create(string name, Type argsType, MemberDataDefinition memberDefinition)
        => new(name, argsType, memberDefinition);

    private DataValue(string name, Type argsType, MemberDataDefinition memberDefinition)
    {
        Name = name;
        ArgsType = argsType;
        MemberDefinition = memberDefinition;
    }

    public string Name { get; }
    public Type ArgsType { get; }
    public MemberDataDefinition MemberDefinition { get; }
    public TValue? Value { get; private set; }
    public DataProvider? SetBy { get; private set; }

    public bool IsSet => SetBy is not null;

    public void SetValue(TValue value, DataProvider setBy)
    {
        SetBy = setBy;
        Value = value;
    }
}
