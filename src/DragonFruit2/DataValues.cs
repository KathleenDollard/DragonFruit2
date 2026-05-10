using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public abstract class DataValues : IEnumerable<DataValue>
{
    readonly Dictionary<MemberDataDefinition, DataValue> _values = [];

    protected DataValues()
    {
        //CommandDefinition = commandDefinition;
    }

    //public CommandDataDefinition CommandDefinition { get; }

    protected void Add<TValue>(DataValue<TValue> value)
    {
        _values[value.MemberDefinition] = value;
    }

    //public bool TryGetValue<TValue>(string name, [NotNullWhen(true)] ref DataValue<TValue>? dataValue)
    //{
    //    if (_values.TryGetValue(name, out var existing) && existing is DataValue<TValue> typed)
    //    {
    //        dataValue = typed;
    //        return true;
    //    }
    //    dataValue = null;
    //    return false;
    //}

    public IEnumerator<DataValue> GetEnumerator()
    {
        return _values.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public abstract class DataValues<TRootCommand> : DataValues
{
    protected DataValues()
        : base()
    { }

    public abstract bool Operate<TReturn>(IOperateOnDataValue<TRootCommand, TReturn> operationContainer);

    protected internal abstract TRootCommand CreateInstance();
    
}
