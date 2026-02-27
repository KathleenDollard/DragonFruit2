using DragonFruit2.Validators;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public class Builder<TRootArgs>
    where TRootArgs : ArgsRootBase<TRootArgs>
{

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
        var result = new Result<TRootArgs>(commandLineArguments);
        try
        {
            CommandLineArguments = commandLineArguments;

            InitializeDataProviders(result);
            if (!TryGetActiveCommandDefinition(result, out var activeCommandDefinition, out var activeDataProvider))
            {
                result.AddDiagnostic(new Diagnostic(DiagnosticId.NoActiveCommand.ToValidationIdString(), DiagnosticSeverity.Error, null, "No active command could be determined."));
                return result;
            }
            result.ActiveCommandDefinition = activeCommandDefinition;
            if (result.ActiveDataProvider is null)

                result.ActiveDataProvider = activeDataProvider;
            if (result.DataValues is null) throw new InvalidOperationException("DataValues should not be null after ActiveCommandDefinition is set");

            GatherActiveDataValues(result);
            GatherDefaultValues(result);
            if (CheckRequired(result) && Validate(result))
            {
                var instance = result.DataValues.CreateInstance();
                result.Args = instance;
            }

            return result;
        }
        catch (Exception e)
        {
            result.AddDiagnostic(new Diagnostic(DiagnosticId.UnexpectedException.ToValidationIdString(), DiagnosticSeverity.Error,
                Message: $"""
                Unexpected exception: 
                   {e.Message}
                """));
            return result;
        }
    }

    public static string[] GetArgsFromEnvironment()
    {
        return [.. Environment.GetCommandLineArgs().Skip(1)];
    }

    private bool TryGetActiveCommandDefinition(Result<TRootArgs> result,
                                               [NotNullWhen(true)] out CommandDataDefinition<TRootArgs> activeCommandDefinition,
                                               [NotNullWhen(true)] out DataProvider<TRootArgs> activeDataProvider)
    {
        foreach (var dataProvider in DataProviders.OfType<IActiveArgsProvider<TRootArgs>>())
        {
            if (dataProvider.TryGetActiveArgsDefinition(result, out activeCommandDefinition, out activeDataProvider))
            {
                return true;
            }
        }
        activeCommandDefinition = null!;
        activeDataProvider = null!;
        return false;
    }

    private void GatherActiveDataValues(Result<TRootArgs> result)
    {

        result.DataValues?.Operate(new SetActiveDataValueOperation(result, result.ActiveDataProvider));
    }

    internal struct SetActiveDataValueOperation : IOperateOnDataValue<TRootArgs, Void>
    {
        private readonly DataProvider<TRootArgs> _dataProvider;
        public SetActiveDataValueOperation(Result<TRootArgs> result, DataProvider<TRootArgs> dataProvider)
        {
            Result = result;
            _dataProvider = dataProvider;
        }

        public Result<TRootArgs> Result { get; init; }
        public string OperationName => nameof(SetActiveDataValueOperation);

        public bool TryOperate<TValue>(DataValue<TValue> dataValue,
                                       IOperateOnDataValue<TRootArgs, Void> operation,
                                       out Void _)
        {
            _ = default;
            if (dataValue is not null && !dataValue.IsSet)
            {
                _dataProvider.TrySetDataValue(dataValue, Result);
                return true;
            }
            return false;
        }
    }

    private void GatherDefaultValues(Result<TRootArgs> result)
    {
        result.DataValues?.Operate(new SetDefaultValueOperation(result, result.ActiveDataProvider, DataProviders));
    }

    internal struct SetDefaultValueOperation : IOperateOnDataValue<TRootArgs, Void>
    {
        private readonly IEnumerable<DataProvider<TRootArgs>> _dataProviders;
        private readonly DataProvider<TRootArgs> _activeDataProvider;

        public SetDefaultValueOperation(Result<TRootArgs> result, DataProvider<TRootArgs> activeDataProvider, IEnumerable<DataProvider<TRootArgs>> dataProviders)
        {
            Result = result;
            _dataProviders = dataProviders;
            _activeDataProvider = activeDataProvider;
        }

        public Result<TRootArgs> Result { get; init; }
        public readonly string OperationName => nameof(SetActiveDataValueOperation);

        public bool TryOperate<TValue>(DataValue<TValue> dataValue,
                                       IOperateOnDataValue<TRootArgs, Void> operation,
                                       out Void _)
        {
            _ = default;
            var activeDataProvider = _activeDataProvider;
            var remainingDataProviders = _dataProviders.Where(dp => dp != activeDataProvider);
            if (dataValue is not null && !dataValue.IsSet)
            {
                foreach (var dataProvider in _dataProviders)
                {
                dataProvider.TrySetDataValue(dataValue, Result);
                return true;

                }
            }
            return false;
        }
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
            if (!dataValue.Validate())
            { isValid = false; }
        }
        return isValid;
    }

    private void InitializeDataProviders(Result<TRootArgs> result)
    {
        foreach (var dataProvider in DataProviders)
        {
            dataProvider.Initialize(this, RootCommandDefinition, result);
        }
    }
}


