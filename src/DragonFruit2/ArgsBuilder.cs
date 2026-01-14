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
   where TRootArgs : class, IArgs<TRootArgs>
{
    //protected static CliDataProvider<TRootArgs> GetCliDataProvider(Builder<TRootArgs> builder)
    //{
    //    var cliDataProvider = builder.DataProviders.OfType<CliDataProvider<TRootArgs>>().FirstOrDefault();
    //    if (cliDataProvider is null)
    //    {
    //        cliDataProvider = new CliDataProvider<TRootArgs>(builder);
    //        builder.DataProviders.Add(cliDataProvider);
    //    }
    //    return cliDataProvider;
    //}

    public abstract void Initialize(Builder<TRootArgs> builder);
    public abstract Command InitializeCli(Builder<TRootArgs> builder, CliDataProvider<TRootArgs>? cliDataProvider);
    protected abstract TRootArgs CreateInstance(Builder<TRootArgs> builder);

    protected abstract IEnumerable<ValidationFailure> CheckRequiredValues(Builder<TRootArgs> builder);

    public Result<TRootArgs> CreateArgs(Builder<TRootArgs> builder, IEnumerable<ValidationFailure>? existingFailures)
    {
        existingFailures ??= Enumerable.Empty<ValidationFailure>();
        var currentFailures = existingFailures.Concat(CheckRequiredValues(builder));

        TRootArgs? args = null;
        if (!currentFailures.Any(x=>x.Severity == DiagnosticSeverity.Error))
        {
            args = CreateInstance(builder);
        }

        var result = new Result<TRootArgs>(currentFailures, args);

        if (result.Args is not null)
        {
            result.AddFailures(result.Args.Validate());
        }

        return result;
    }

    protected ValidationFailure? CheckRequiredValue<TValue>(string valueName, DataValue<TValue> dataValue)
    {
        if (!dataValue.IsSet || dataValue.Value is null)
        {
            var message = $"The value for {valueName} is required but was not provided.";
            var idString = $"{Validator.ToValidationIdString(ValidationId.Required)};";
            return new ValidationFailure<TValue>(idString, message, valueName, DiagnosticSeverity.Error, dataValue.Value);
        }
        return null;
    }
}
