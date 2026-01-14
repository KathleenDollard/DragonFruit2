using DragonFruit2.Validators;

namespace DragonFruit2;

public class Builder<TRootArgs>
    where TRootArgs : class, IArgs<TRootArgs>
{
    public Builder(string[] commandLineArguments, DragonFruit2Configuration? configuration = null)
    {
        CommandLineArguments = commandLineArguments;
        AddDataProvider(new CliDataProvider<TRootArgs>(this));
        Configuration = configuration;
    }

    public string[] CommandLineArguments { get; }
    public List<DataProvider> DataProviders { get; } = [];
    public DragonFruit2Configuration? Configuration { get; }

    public void AddDataProvider(DataProvider provider, int position = int.MaxValue)
    {
        // TODO: Should we protect against multiple entries of the same provider? The same provider type? (might be scenarios for that) Have an "allow multiples" trait on the provider? (How would we do that in Framework?) Have each provider build a key that could differentiate?
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

    public Result<TRootArgs> ParseArgs(ArgsBuilder<TRootArgs> argsBuilder)
    {
        argsBuilder.Initialize(this);

        var cliDataProvider = DataProviders.OfType<IActiveArgsBuilderProvider<TRootArgs>>().FirstOrDefault()
            ?? throw new InvalidOperationException("Internal error: CliDataProvider not found");
        // Once you set the InputArgs, the provider can start parsing
        var (failures, activeArgsBuilder) = cliDataProvider.GetActiveArgsBuilder();

        return activeArgsBuilder is null
                    ? new Result<TRootArgs>(failures, null)
                    : activeArgsBuilder.CreateArgs(this, failures);
    }
}
