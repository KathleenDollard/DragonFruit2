namespace DragonFruit2;

public class DefaultDataProvider<TRootArgs> : DataProvider<TRootArgs>
     where TRootArgs : ArgsRootBase<TRootArgs>

{
    public DefaultDataProvider(Builder<TRootArgs> builder)
        : base(builder)
    {
    }

    private readonly Dictionary<(Type argsType, string propertyName), object> defaultValues = new();

    public override bool TryGetValue<TValue>((Type argsType, string propertyName) key, DataValue<TValue> dataValue)
    {
        var memberDefinition = dataValue.MemberDefinition;

        var defaultValues = memberDefinition.TryGetDefault(dataValues);



    }

    public void RegisterDefault<TValue>(Type argsType, string propertyName, TValue value)
    {
        if (value is not null && !value.Equals(default(TValue)))
        {
            defaultValues[(argsType, propertyName)] = value;
        }
    }
}
