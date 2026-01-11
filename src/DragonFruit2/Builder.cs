namespace DragonFruit2;

public class Builder<TArgs>
    where TArgs : Args<TArgs>, IArgs<TArgs>
{
    public Builder()
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


    public DataValues<TArgs> ParseArgs(string[] args)
    {
        var dataBuilder  = TArgs.GetArgsBuilder(this);
        dataBuilder.Initialize(this);

        var cliDataProvider = DataProviders.OfType<CliDataProvider<TArgs>>().FirstOrDefault()
            ?? throw new InvalidOperationException("Internal error: CliDataProvider not found");
        cliDataProvider.InputArgs = args;
        var argsDataValues = dataBuilder.Create(this);
        return argsDataValues;
    }
}
