using DragonFruit2.Validators;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public class Builder<TRootArgs>
    where TRootArgs : ArgsRootBase<TRootArgs>
{

    public static string[] GetArgsFromEnvironment()
    {
        return [.. Environment.GetCommandLineArgs().Skip(1)];
    }

    public Builder(CommandDataDefinition<TRootArgs> rootCommandDefinition, DragonFruit2Configuration? configuration = null)
    {
        AddDataProvider(new CliDataProvider<TRootArgs>(this));
        AddDataProvider(new DefaultDataProvider<TRootArgs>(this));
        Configuration = configuration;
        RootCommandDefinition = rootCommandDefinition;
    }

    public CommandDataDefinition<TRootArgs> RootCommandDefinition { get; }

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

    public Result<TRootArgs> ParseArgs(string[]? commandLineArguments)
    {
        commandLineArguments ??= GetArgsFromEnvironment();
        CommandLineArguments = commandLineArguments;

        InitializeDataProviders();
        var result = new Result<TRootArgs>(commandLineArguments);
        if (!TryGetActiveCommandDefinition(result, out var activeCommandDefinition))
        {
            result.AddDiagnostic(new Diagnostic(DiagnosticId.NoActiveCommand.ToValidationIdString(), DiagnosticSeverity.Error, null, "No active command could be determined."));
            return result;
        }
        result.ActiveCommandDefinition = activeCommandDefinition;
        if (result.DataValues is null) throw new InvalidOperationException("DataValues should not be null after ActiveCommandDefinition is set");

        GatherDataValues(result);
        if (CheckRequired(result) && Validate(result))
        {
            var instance = result.DataValues.CreateInstance();
            result.Args = instance;
        }
        return result;
    }

    private bool CheckRequired(Result<TRootArgs> result)
    {
        if (result.ActiveCommandDefinition is null) throw new ArgumentNullException(nameof(result.ActiveCommandDefinition));
        if (result.DataValues is null) throw new ArgumentNullException(nameof(result.DataValues));

        foreach (var dataValue in result.DataValues)
        {
            if (dataValue.MemberDefinition.IsRequired && !dataValue.IsSet)
            {
                result.AddDiagnostic(new Diagnostic(DiagnosticId.Required.ToValidationIdString(), DiagnosticSeverity.Error, dataValue.MemberDefinition.DefinitionName));
                return false;
            }
        }
        return true;
    }

    private bool Validate(Result<TRootArgs> result)
    {
        if (result.ActiveCommandDefinition is null) throw new ArgumentNullException(nameof(result.ActiveCommandDefinition));
        if (result.DataValues is null) throw new ArgumentNullException(nameof(result.DataValues));

        var isValid = true;
        foreach (var dataValue in result.DataValues)
        {
            if (! dataValue.Validate())
            { isValid = false; }
        }
        return isValid;
    }

    private void GatherDataValues(Result<TRootArgs> result)
    {
        if (result.ActiveCommandDefinition is null)
        {
            throw new ArgumentNullException(nameof(result.ActiveCommandDefinition));
        }
        // TPDO: This is called after a null check on result.DataValues. Are we setting DataValues twice?
        result.DataValues = result.ActiveCommandDefinition.CreateDataValues();
        foreach (var dataProvider in DataProviders)
        {
            result.DataValues.SetDataValues(dataProvider, result);
        }
    }

    private bool TryGetActiveCommandDefinition(Result<TRootArgs> result, [NotNullWhen(true)] out CommandDataDefinition<TRootArgs> activeCommandDefinition)
    {
        foreach (var dataProvider in DataProviders.OfType<IActiveArgsProvider<TRootArgs>>())
        {
            if (dataProvider.TryGetActiveArgsDefinition(result, out activeCommandDefinition))
            {
                return true;
            }
        }
        activeCommandDefinition = null!;
        return false;
    }

    private void InitializeDataProviders()
    {
        foreach (var dataProvider in DataProviders)
        {
            dataProvider.Initialize(this, RootCommandDefinition);
        }
    }
}


