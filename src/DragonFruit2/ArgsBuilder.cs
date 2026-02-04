//using DragonFruit2.Validators;
//using System.CommandLine;

//namespace DragonFruit2;

///// <summary>
///// This type provides a common type to store ArgBuilders in the cache
///// </summary>
//public abstract class ArgsBuilder
//{

//}

///// <summary>
///// 
///// </summary>
///// <typeparam name="TRootArgs"></typeparam>
///// <typeparam name="TRootArgs"></typeparam>
//public abstract class ArgsBuilder<TRootArgs> : ArgsBuilder
//   where TRootArgs : ArgsRootBase<TRootArgs>
//{

//    public abstract void Initialize(Builder<TRootArgs> builder);
//    public abstract Command InitializeCli(Builder<TRootArgs> builder, CliDataProvider<TRootArgs>? cliDataProvider);
//    protected abstract IEnumerable<Diagnostic> CheckRequiredValues(DataValues dataValues);

//    private readonly Func<DataValues<TRootArgs>> _createDataValues;

//    protected ArgsBuilder(CommandDataDefinition commandDataDefintion, Func<DataValues<TRootArgs>> createDataValues)
//    {
//        CommandDataDefinition = commandDataDefintion;
//        _createDataValues = createDataValues;
//    }

//    public readonly CommandDataDefinition CommandDataDefinition;

//    public Builder<TRootArgs>? Builder
//    {
//        get;
//        set
//        {
//            if (field is not null) throw new InvalidOperationException("The Builder only be set once");
//        }
//    }

//    public Result<TRootArgs> CreateArgs(Builder<TRootArgs> builder, IEnumerable<Diagnostic>? existingFailures)
//    {
//        existingFailures ??= Enumerable.Empty<Diagnostic>();
//        var dataValues = _createDataValues();
//        foreach (var dataProvider in builder.DataProviders)
//        {
//            dataValues.SetDataValues(dataProvider);
//        }

//        var currentFailures = existingFailures.Concat(CheckRequiredValues(dataValues));

//        TRootArgs? args = null;
//        if (!currentFailures.Any(x => x.Severity == DiagnosticSeverity.Error))
//        {
//            args = dataValues.CreateInstance();
//        }

//        var result = new Result<TRootArgs>(currentFailures, args);

//        if (result.Args is not null)
//        {
//            result.AddFailures(result.Args.Validate());
//        }

//        return result;
//    }


//    protected void AddFailureIfNeeded<TValue>(List<Diagnostic> validationFailures, bool hasFailed, TValue value, string valueName, string idString, string message)
//    {
//        // TODO: Replace Id and message with closer adherence to Roslyn diagnostic patterns
//        if (hasFailed)
//        {
//            validationFailures.Add(new ValidationFailure<TValue>(idString, message, valueName, DiagnosticSeverity.Error, value));
//        }
//    }

//    protected void AddRequiredFailureIfNeeded<TValue>(List<Diagnostic> validationFailures, bool hasFailed, string valueName)
//    {
//        // TODO: Replace Id and message with closer adherence to Roslyn diagnostic patterns
//        if (hasFailed)
//        {
//            validationFailures.Add(new ValidationFailure(DiagnosticId.Required.ToValidationIdString(),$"Required value {valueName} noy provided.", valueName, DiagnosticSeverity.Error));
//        }
//    }
//}
