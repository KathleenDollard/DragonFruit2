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
        if (defaultValues.TryGetValue(key, out var value))
        {
            if (value is TValue retrievedValue)
            {
                dataValue.SetValue(retrievedValue, this);
                return true;
            }
            throw new InvalidOperationException("Issue with the default values lookup.");
        }
        return false;
    }

    public void RegisterDefault<TValue>(Type argsType, string propertyName, TValue value)
    {
        if (value is not null && !value.Equals(default(TValue)))
        {
            defaultValues[(argsType, propertyName)] = value;
        }
    }

    public override void Initialize(Builder<TRootArgs> builder, CommandDataDefinition<TRootArgs> commandDefinition)
    {
        //throw new NotImplementedException();
    }
}
