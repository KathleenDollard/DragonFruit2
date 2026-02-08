using DragonFruit2.Defaults;
using DragonFruit2.Validators;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

/// <summary>
/// Represents the definition of a data member corresponding to a property in an ArgsClass.
/// </summary>
/// <remarks>
/// This class may be frequently used by data providers that have no interest in the CLI shape.
/// </remarks>
public abstract class MemberDataDefinition : DataDefinition
{
    public abstract IEnumerable<Diagnostic>? Validate(DataValue dataValue);

    public MemberDataDefinition(CommandDataDefinition commandDefinition, string name, bool isOption)
        : base(name)
    {
        CommandDefinition = commandDefinition;
        IsOption = isOption;
    }

    public CommandDataDefinition CommandDefinition { get; }
    public required Type DataType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsOption { get; }
}

public class MemberDataDefinition<TValue> : MemberDataDefinition
{
    private readonly List<DefaultDefinition<TValue>> _defaultDefinitions = [];
    public readonly List<Validator<TValue>> _validators = [];

    public MemberDataDefinition(CommandDataDefinition commandDefinition, string name, bool isOption)
        : base(commandDefinition, name, isOption)
    { }

    public IEnumerable<DefaultDefinition<TValue>> Defaults => _defaultDefinitions;
    public IEnumerable<Validator<TValue>> Validators => _validators;

    public bool TryGetDefault(DataValues dataValues, [NotNullWhen(true)] out TValue value)
    {
        foreach (var defaultDefinition in Defaults)
        {
            if (defaultDefinition.TryGetDefaultValue(dataValues, out value))
            { return true; }
        }
        value = default!;
        return false;
    }

    public override IEnumerable<Diagnostic>? Validate(DataValue dataValue)
    {
        if (!(dataValue is DataValue<TValue> typedDataValue))
        {
            throw new InvalidOperationException($"Data values is of an unexpected type. The expect type is {typeof(TValue)}, but the passed type is {dataValue.GetType().GenericTypeArguments.First()}");
        }

        List<Diagnostic>? diagnostics =null;
        foreach (var validator in Validators)
        {
            var newDiagnostics =  validator.Validate(typedDataValue);
            if (newDiagnostics is not null && newDiagnostics.Any())
            {
                diagnostics ??= new List<Diagnostic>();
                diagnostics.AddRange(newDiagnostics);
            }
        }
        return diagnostics;
    }

    public void Default(DefaultDefinition<TValue> defaultDefinition)
    {
        _defaultDefinitions.Add(defaultDefinition);
    }
    public void Default(TValue value)
    {
        _defaultDefinitions.Add(DefaultConstant<TValue>.Create(value));
    }

    public void RegisterValidator(Validator<TValue> validator)
    {
        _validators.Add(validator);
    }

    public void RegisterAsRequired(bool required = true)
    {
        IsRequired = required;
    }
}