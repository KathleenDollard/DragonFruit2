using DragonFruit2.Validators;
using System.CommandLine;

namespace DragonFruit2;

/// <summary>
/// This type provides a common type to store ArgBuilders in the cache
/// </summary>
public abstract class ArgsBuilder
{

}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRootArgs"></typeparam>
/// <typeparam name="TRootArgs"></typeparam>
public abstract class ArgsBuilder<TRootArgs> : ArgsBuilder
   where TRootArgs : ArgsRootBase<TRootArgs>
{

    public abstract void Initialize(Builder<TRootArgs> builder);
    public abstract Command InitializeCli(Builder<TRootArgs> builder, CliDataProvider<TRootArgs>? cliDataProvider);
    protected abstract TRootArgs CreateInstance(DataValues dataValues);
    protected abstract IEnumerable<ValidationFailure> CheckRequiredValues(DataValues dataValues);
    protected abstract DataValues<TRootArgs> CreateDataValues();

    public Builder<TRootArgs>? Builder
    {
        get;
        set
        {
            if (field is not null) throw new InvalidOperationException("The Builder only be set once");
        }
    }

    public Result<TRootArgs> CreateArgs(Builder<TRootArgs> builder, IEnumerable<ValidationFailure>? existingFailures)
    {
        existingFailures ??= Enumerable.Empty<ValidationFailure>();
        var dataValues = CreateDataValues();
        foreach (var dataProvider in builder.DataProviders)
        {
            dataValues.SetDataValues(dataProvider);
        }

        var currentFailures = existingFailures.Concat(CheckRequiredValues(dataValues));

        TRootArgs? args = null;
        if (!currentFailures.Any(x => x.Severity == DiagnosticSeverity.Error))
        {
            args = CreateInstance(dataValues);
        }

        var result = new Result<TRootArgs>(currentFailures, args);

        if (result.Args is not null)
        {
            result.AddFailures(result.Args.Validate());
        }

        return result;
    }

    //protected ValidationFailure? CheckRequiredValue<TValue>(string valueName, DataValue<TValue>? dataValue)
    //{
    //    if (dataValue is null || !dataValue.IsSet)
    //    {
    //        var message = $"The value for {valueName} is required but was not provided.";
    //        var idString = $"{Validator.ToValidationIdString(ValidationId.Required)};";
    //        return new ValidationFailure<TValue>(idString, message, valueName, DiagnosticSeverity.Error, dataValue.Value);
    //    }
    //    return null;
    //}

    protected void AddFailureIfNeeded<TValue>(List<ValidationFailure> validationFailures, bool hasFailed, TValue value, string valueName, string idString, string message)
    {
        // TODO: Replace Id and message with closer adherence to Roslyn diagnostic patterns
        if (hasFailed)
        {
            validationFailures.Add(new ValidationFailure<TValue>(idString, message, valueName, DiagnosticSeverity.Error, value));
        }
    }

    protected void AddRequiredFailureIfNeeded<TValue>(List<ValidationFailure> validationFailures, bool hasFailed, string valueName)
    {
        // TODO: Replace Id and message with closer adherence to Roslyn diagnostic patterns
        if (hasFailed)
        {
            validationFailures.Add(new ValidationFailure(ValidationId.Required.ToValidationIdString(),$"Required value {valueName} noy provided.", valueName, DiagnosticSeverity.Error));
        }
    }
}
