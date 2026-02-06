using DragonFruit2;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public abstract class DataValues
{
    Dictionary<string, DataValue> values = [];

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
    public abstract void SetDataValues(DataProvider<TRootArgs> dataProvider, Result<TRootArgs> result);

    protected internal abstract TRootArgs CreateInstance();
}
