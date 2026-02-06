using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public abstract class DataValues
{
    Dictionary<string, DataValue> values = [];

    protected DataValues(CommandDataDefinition commandDefinition)
    {
        CommandDefinition = commandDefinition;
    }

    public CommandDataDefinition CommandDefinition { get; }

    protected void Add<TValue>(DataValue<TValue> value)
    {
        values[value.Name] = value;
    }

    public bool TryGetValue<TValue>(string name,[NotNullWhen(true)] out DataValue<TValue>? value)
    {
        if (values.TryGetValue(name, out var existing) && existing is DataValue<TValue> typed)
        {
            value = typed;
            return true;
        }
        value = null;
        return false;
    }
}

public abstract class DataValues<TRootArgs> : DataValues
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    protected DataValues(CommandDataDefinition commandDefinition)
        :base (commandDefinition)
    {  }

    public abstract void SetDataValues(DataProvider<TRootArgs> dataProvider, Result<TRootArgs> result);

    protected internal abstract TRootArgs CreateInstance();
}
