namespace DragonFruit2.Common;

public abstract class DataProvider
{
    public abstract bool TryGetValue<TValue>(string key, object[] alternateKeys, out DataValue<TValue> value);
}
