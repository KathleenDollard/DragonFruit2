using DragonFruit2.Validators;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

public class Builder<TRootCommand>
{

    public Builder(CommandDataDefinition<TRootCommand> rootCommandDefinition, DragonFruit2Configuration? configuration = null)
    {
        AddDataProvider(new CliDataProvider<TRootCommand>(this));
        AddDataProvider(new DefaultDataProvider<TRootCommand>(this));
        Configuration = configuration;
        RootCommandDefinition = rootCommandDefinition;
    }

    public CommandDataDefinition<TRootCommand> RootCommandDefinition { get; }

    public string[]? CommandLineArguments { get; protected set; }

    public List<DataProvider<TRootCommand>> DataProviders { get; } = [];
    public DragonFruit2Configuration? Configuration { get; }

    public TDataProvider GetDataProvider<TDataProvider>()
            where TDataProvider : DataProvider<TRootCommand>
        => DataProviders.OfType<TDataProvider>().FirstOrDefault();

    public void AddDataProvider(DataProvider<TRootCommand> provider, int position = int.MaxValue)
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

    public Result<TRootCommand> ParseArgs(string[]? commandLineArguments)
    {
        commandLineArguments ??= GetArgsFromEnvironment();
        var result = new Result<TRootCommand>(commandLineArguments);
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
            result.ActiveDataProvider ??= activeDataProvider;
            if (result.DataValues is null) throw new InvalidOperationException("DataValues should not be null after ActiveCommandDefinition is set");

            GatherActiveDataValues(result);
            GatherFromOtherDataProviders(result);
            if (Validate(result))
            {
                var instance = result.DataValues.CreateInstance();
                result.Command = instance;
            }

            return result;
        }
        catch (Exception e)
        {
            // TODO: Create special diagnostic that includes all exception info
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

    private bool TryGetActiveCommandDefinition(Result<TRootCommand> result,
                                               [NotNullWhen(true)] out CommandDataDefinition<TRootCommand> activeCommandDefinition,
                                               [NotNullWhen(true)] out DataProvider<TRootCommand> activeDataProvider)
    {
        foreach (var dataProvider in DataProviders.OfType<IActiveCommandProvider<TRootCommand>>())
        {
            if (dataProvider.TryGetActiveCommandDefinition(result, out activeCommandDefinition, out activeDataProvider))
            {
                return true;
            }
        }
        activeCommandDefinition = null!;
        activeDataProvider = null!;
        return false;
    }

    private void GatherActiveDataValues(Result<TRootCommand> result)
    {

        result.DataValues?.Operate(new SetActiveDataValueOperation(result, result.ActiveDataProvider));
    }

    internal readonly struct SetActiveDataValueOperation : IOperateOnDataValue<TRootCommand, Void>
    {
        private readonly DataProvider<TRootCommand> _dataProvider;
        public SetActiveDataValueOperation(Result<TRootCommand> result, DataProvider<TRootCommand> dataProvider)
        {
            Result = result;
            _dataProvider = dataProvider;
        }

        public Result<TRootCommand> Result { get; init; }
        public string OperationName => nameof(SetActiveDataValueOperation);

        public bool TryOperate<TValue>(DataValue<TValue> dataValue,
                                       IOperateOnDataValue<TRootCommand, Void> operation,
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

    private void GatherFromOtherDataProviders(Result<TRootCommand> result)
    {
        result.DataValues?.Operate(new GatherFromOtherDataProvidersOperation(result, result.ActiveDataProvider, DataProviders));
    }

    internal readonly struct GatherFromOtherDataProvidersOperation : IOperateOnDataValue<TRootCommand, Void>
    {
        private readonly IEnumerable<DataProvider<TRootCommand>> _dataProviders;
        private readonly DataProvider<TRootCommand> _activeDataProvider;

        public GatherFromOtherDataProvidersOperation(Result<TRootCommand> result, DataProvider<TRootCommand> activeDataProvider, IEnumerable<DataProvider<TRootCommand>> dataProviders)
        {
            Result = result;
            _dataProviders = dataProviders;
            _activeDataProvider = activeDataProvider;
        }

        public Result<TRootCommand> Result { get; init; }
        public readonly string OperationName => nameof(SetActiveDataValueOperation);

        public bool TryOperate<TValue>(DataValue<TValue> dataValue,
                                       IOperateOnDataValue<TRootCommand, Void> operation,
                                       out Void _)
        {
            _ = default;
            var activeDataProvider = _activeDataProvider;
            var remainingDataProviders = _dataProviders.Where(dp => dp != activeDataProvider);
            if (dataValue is not null && !dataValue.IsSet)
            {
                foreach (var dataProvider in _dataProviders)
                {
                    if (dataProvider.TrySetDataValue(dataValue, Result))
                    {
                        return true;
                    }

                }
            }
            return false;
        }
    }



    //private bool CheckRequired(Result<TRootCommand> result)
    //{
    //    if (result.ActiveCommandDefinition is null) throw new ArgumentNullException(nameof(result.ActiveCommandDefinition));
    //    if (result.DataValues is null) throw new ArgumentNullException(nameof(result.DataValues));

    //    foreach (var dataValue in result.DataValues)
    //    {
    //        if (dataValue.MemberDefinition.IsRequired && !dataValue.IsSet)
    //        {
    //            result.AddDiagnostic(new Diagnostic(DiagnosticId.Required.ToValidationIdString(),
    //                                                DiagnosticSeverity.Error,
    //                                                dataValue.MemberDefinition.DefinitionName,
    //                                                $"Required value not supplied: {dataValue.MemberDefinition.DefinitionName}"));
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    private bool Validate(Result<TRootCommand> result)
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

    private void InitializeDataProviders(Result<TRootCommand> result)
    {
        foreach (var dataProvider in DataProviders)
        {
            dataProvider.Initialize(this, RootCommandDefinition, result);
        }
    }
}


