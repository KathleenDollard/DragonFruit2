using DragonFruit2.Validators;
using System.CommandLine;

namespace DragonFruit2;

/// <summary>
/// This type provides a common type to store ArgBuilders in the cache
/// </summary>
public abstract class ArgsBuilder
{

}

///// <summary>
///// This type provides a common type to store ArgBuilders in the cache
///// </summary>
//public abstract class ArgsBuilder<TRootArgs> : ArgsBuilder
//    where TRootArgs : Args<TRootArgs>
//{

//}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRootArgs"></typeparam>
/// <typeparam name="TRootArgs"></typeparam>
public abstract class ArgsBuilder<TRootArgs> : ArgsBuilder
   where TRootArgs : IArgs<TRootArgs>
{
    protected static CliDataProvider<TRootArgs> GetCliDataProvider(Builder<TRootArgs> builder)
    {
        var cliDataProvider = builder.DataProviders.OfType<CliDataProvider<TRootArgs>>().FirstOrDefault();
        if (cliDataProvider is null)
        {
            cliDataProvider = new CliDataProvider<TRootArgs>();
            builder.DataProviders.Add(cliDataProvider);
        }
        return cliDataProvider;
    }

    public abstract Command Initialize(Builder<TRootArgs> builder);

    protected abstract TRootArgs CreateInstance(Builder<TRootArgs> builder);

    protected abstract IEnumerable<ValidationFailure> CheckRequiredValues(Builder<TRootArgs> builder);

    public DataValues<TRootArgs> CreateArgs(Builder<TRootArgs> builder)
    {
        var dataValues = new DataValues<TRootArgs>();
        dataValues.ValidationFailures.AddRange(CheckRequiredValues(builder));
        if (dataValues.IsValid)
        {
            var newArgs = CreateInstance(builder);
            dataValues.Args = newArgs;
        }

        if (dataValues.Args is not null)
        {
            dataValues.ValidationFailures.AddRange(dataValues.Args.Validate());
        }

        return dataValues;
    }

    protected ValidationFailure? CheckRequiredValue<TValue>(string valueName, DataValue<TValue> dataValue)
    {
        if (!dataValue.IsSet || dataValue.Value is null)
        {
            var message = $"The value for {valueName} is required but was not provided.";
            var idString = $"{Validator.ToValidationIdString(ValidationId.Required)};";
            return new ValidationFailure<TValue>(idString, message, valueName, dataValue.Value);
        }
        return null;
    }
}
