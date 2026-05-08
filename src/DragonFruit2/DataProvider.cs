namespace DragonFruit2;

public abstract class DataProvider
{
}

public abstract class DataProvider<TRootCommand> : DataProvider
{
    protected DataProvider(Builder<TRootCommand> builder)
    {
        Builder = builder;
    }

    public Builder<TRootCommand> Builder { get; }

    protected abstract bool TryGetValue<TValue>(MemberDataDefinition<TValue> memberDefinition, Result<TRootCommand> result, out TValue Value);

    public bool TrySetDataValue<TValue>(DataValue<TValue> dataValue, Result<TRootCommand> result)
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

    public virtual void Initialize(Builder<TRootCommand> builder, CommandDataDefinition<TRootCommand> commandDefinition, Result<TRootCommand> result)
    {
        // no op
    }
}
