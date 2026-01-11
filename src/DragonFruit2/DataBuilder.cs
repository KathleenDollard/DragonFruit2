using DragonFruit2.Validators;

namespace DragonFruit2;

public abstract class ArgsBuilder<TArgs> where TArgs : Args<TArgs>, IArgs<TArgs>
{
    protected CliDataProvider<TArgs> GetCliDataProvider(Builder<TArgs> builder)
    {
        var cliDataProvider = builder.DataProviders.OfType<CliDataProvider<TArgs>>().FirstOrDefault();
        if (cliDataProvider is null)
        {
            cliDataProvider = new CliDataProvider<TArgs>();
            builder.DataProviders.Add(cliDataProvider);
        }
        return cliDataProvider;
    }
    public abstract void Initialize(Builder<TArgs> builder);

    public DataValues<TArgs> Create(Builder<TArgs> builder)
    {
        // This seems the place to determine which subcommand to call
        // * Args should know if they have subcommands, probably root knowing all the subcommands from the root
        // * Assuming the root ArgsBuilder has a symbol/command switch or a dictionary of Func<TArgs>
        // * Once we know the actual args subcommand class, and can instantiate get its argsbuilder, we 
        //   can work up the inheritance hiearchy for any prpoerties that are shared
        //
        // Another way to solve this is to have Actions on the SCL.Commands that supply the ArgsBuilder
        // * That might let us avoid the IArgs interface and support C# 7.1
        // * The CliDataProvider is already a flawed class because it holds data (not reentrant) so we just extend that
        var dataValues = new DataValues<TArgs>();
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

    protected abstract TArgs CreateInstance(Builder<TArgs> builder);
    protected abstract IEnumerable<ValidationFailure> CheckRequiredValues(Builder<TArgs> builder);

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
