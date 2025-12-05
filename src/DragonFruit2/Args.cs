namespace DragonFruit2;

public abstract class Args
{
    public static List<DataProvider> DataProviders { get; } = [];

    public static void AddDataProvider(DataProvider provider, int position = int.MaxValue)
    {
        if (position < int.MaxValue)
        {
            DataProviders.Insert(position, provider);
        }
        else
        {
            DataProviders.Add(provider);
        }
    }

    public static DataValue<T> GetDataValue<T>(string key, params object[] alternateKeys)
    {
        foreach (var dataProvider in DataProviders)
        {
            if (dataProvider.TryGetValue<T>(key, alternateKeys, out var dataValue))
            {
                return dataValue;
            }
        }
        return DataValue<T>.CreateEmpty();
    }
}
