namespace DragonFruit2;

public abstract class DataProvider
{
}

public abstract class DataProvider<TRootArgs> : DataProvider
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    protected DataProvider(Builder<TRootArgs> builder)
    {
        Builder = builder;
    }

    public Builder<TRootArgs> Builder { get; }

    public abstract bool TryGetValue<TValue>((Type argsType, string propertyName) key, DataValue<TValue> value);

    public bool TrySetDataValue<TValue>((Type argsType, string propertyName) key, DataValue<TValue> dataValue)
    {
        if (TryGetValue(key, dataValue))
        {
            return true;
        }
        return false;
    }

    public virtual void Initialize(Builder<TRootArgs> builder, CommandDataDefinition<TRootArgs> commandDefinition)
    {
        // no op
    }
}
