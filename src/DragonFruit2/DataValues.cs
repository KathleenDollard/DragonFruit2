using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public abstract class DataValues : IEnumerable<DataValue>
{
    readonly Dictionary<string, DataValue> _values = [];

    protected DataValues(CommandDataDefinition commandDefinition)
    {
        CommandDefinition = commandDefinition;
    }

    public CommandDataDefinition CommandDefinition { get; }

    protected void Add<TValue>(DataValue<TValue> value)
    {
        _values[value.Name] = value;
    }

    public bool TryGetValue<TValue>(string name,[NotNullWhen(true)] ref DataValue<TValue>? dataValue)
    {
        if (_values.TryGetValue(name, out var existing) && existing is DataValue<TValue> typed)
        {
            dataValue = typed;
            return true;
        }
        dataValue = null;
        return false;
    }

    public IEnumerator<DataValue> GetEnumerator()
    {
        return _values.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }}

public abstract class DataValues<TRootArgs> : DataValues
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    protected DataValues(CommandDataDefinition commandDefinition)
        :base (commandDefinition)
    {  }

    public abstract bool Operate<TReturn>(IOperateOnDataValue<TRootArgs,TReturn> operationContainer);

    protected internal abstract TRootArgs CreateInstance();


}
