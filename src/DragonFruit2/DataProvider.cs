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

    public abstract bool TryGetValue<TValue>(MemberDataDefinition<TValue> memberDefinition, Result<TRootArgs> result, out TValue Value);

    public bool TrySetDataValue<TValue>(DataValue<TValue> dataValue, Result<TRootArgs> result)
    {
        if (dataValue.IsSet)
        {
            return false;
        }
        if (TryGetValue(dataValue.MemberDefinition, result, out TValue value))
        {
            dataValue.SetValue(value, this);
            return true;
        }
        return false;
    }

    public virtual void Initialize(Builder<TRootArgs> builder, CommandDataDefinition<TRootArgs> commandDefinition, Result<TRootArgs> result)
    {
        // no op
    }
}
