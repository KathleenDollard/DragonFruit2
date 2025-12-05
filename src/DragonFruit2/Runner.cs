using DragonFruit2.Common;

namespace DragonFruit2;

public class Runner<TArgs>
    where TArgs : IArgs<TArgs>
{
    public Runner()
    {
        AddDataProvider(new CliDataProvider<TArgs>());
    }

    public List<DataProvider> DataProviders { get; } = [];

    public void AddDataProvider(DataProvider provider, int position = int.MaxValue)
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

    public DataValue<T> GetDataValue<T>(string key, params object[] alternateKeys)
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

    //public TArgs CreateArgs()
    //{
    //    TArgs.Create();
    //}
}
