using DragonFruit2.Validators;

namespace DragonFruit2;

public class Builder<TRootArgs>
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    public CommandDataDefinition<TRootArgs> RootCommandDefinition { get; }

    public Builder(CommandDataDefinition<TRootArgs> rootCommandDefinition, DragonFruit2Configuration? configuration = null)
    {
        AddDataProvider(new CliDataProvider<TRootArgs>(this));
        AddDataProvider(new DefaultDataProvider<TRootArgs>(this));
        Configuration = configuration;
        RootCommandDefinition = rootCommandDefinition;
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

        // TODO Consider a desigg that does Initialize on every ParseArgs call
        foreach (var dataProvider in DataProviders)
        {
            dataProvider.Initialize(this, RootCommandDefinition);
        }

        var result = new Result<TRootArgs>(args);
        foreach (var dataProvider in DataProviders.OfType<IActiveArgsProvider<TRootArgs>>())
        {
            if (dataProvider.TryGetActiveArgsDefinition(out var activeArgsDefinition))
            {
                result.ActiveCommandDefinition = activeArgsDefinition;
                break;
            }
        }

        if (result.ActiveCommandDefinition is null)
        {
            result.AddDiagnostic(new Diagnostic(DiagnosticId.NoActiveCommand.ToValidationIdString(), "No active command could be determined.", null, DiagnosticSeverity.Error));
            return result;
        }

        result.DataValues = result.ActiveCommandDefinition.CreateDataValues();

        foreach (var dataProvider in DataProviders)
        {
            result.DataValues.SetDataValues(dataProvider);
        }

        // TODO: CheckRequired
        var instance = result.DataValues.CreateInstance();
        result.Args = instance;

        return result;
    }
}


