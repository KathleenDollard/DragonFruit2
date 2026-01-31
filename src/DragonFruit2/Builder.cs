using DragonFruit2;

namespace DragonFruit2;

public class Builder<TRootArgs>
    where TRootArgs :  ArgsRootBase<TRootArgs>
{
    public ArgsBuilder<TRootArgs> RootArgsBuilder { get; }

    public Builder(ArgsBuilder<TRootArgs> rootArgsBuilder, DragonFruit2Configuration? configuration = null)
    {
        AddDataProvider(new CliDataProvider<TRootArgs>(this));
        AddDataProvider(new DefaultDataProvider<TRootArgs>(this));
        Configuration = configuration;
        RootArgsBuilder = rootArgsBuilder;
    }

    public string[]? CommandLineArguments { get; protected set; }

    public List<DataProvider<TRootArgs>> DataProviders { get; } = [];
    public DragonFruit2Configuration? Configuration { get; }

    public TDataProvider GetDataProvider<TDataProvider>()
            where TDataProvider : DataProvider<TRootArgs>
        => DataProviders.OfType<TDataProvider>().FirstOrDefault();

    public void AddDataProvider(DataProvider<TRootArgs> provider, int position = int.MaxValue)
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

    public void SetDataValue<TValue>((Type argsType, string propertyName) key, DataValue<TValue> dataValue)
    {
        foreach (var dataProvider in DataProviders)
        {
            if (dataProvider.TryGetValue(key, dataValue))
            {
                return;
            }
        }
    }

    public Result<TRootArgs> ParseArgs(string[] args)
    {
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
        CommandLineArguments = args;
        RootArgsBuilder.Initialize(this);

        var cliDataProvider = DataProviders.OfType<IActiveArgsBuilderProvider<TRootArgs>>().FirstOrDefault()
            ?? throw new InvalidOperationException("Internal error: CliDataProvider not found");
        // Once you set the InputArgs, the provider can start parsing
        var (failures, activeArgsBuilder) = cliDataProvider.GetActiveArgsBuilder();

        return activeArgsBuilder is null
                    ? new Result<TRootArgs>(failures, null)
                    : activeArgsBuilder.CreateArgs(this, failures);
    }
}


