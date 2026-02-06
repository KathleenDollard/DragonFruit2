namespace DragonFruit2.Validators;

public class GreaterThanValidator<TValue> : Validator<TValue>
    where TValue : IComparable<TValue>
{
    public GreaterThanValidator(string valueName, TValue compareWithValue, string? customMessage = null)
        : base((int)DiagnosticId.GreaterThan, valueName)
    {
        CompareWithValue = compareWithValue;
    }

    public override string Description => $"The value of {ValueName} must be greater than {CompareWithValue}";
    public TValue CompareWithValue { get; }

    public override IEnumerable<Diagnostic<TValue>> Validate(TValue value)
    {
        if (value.CompareTo(CompareWithValue) <= 0)
        {
            var message = $"The value of {ValueName} must be greater than {CompareWithValue}, and {value} is not.";
            return [new Diagnostic<TValue>(Id, message, ValueName, DiagnosticSeverity.Error, value)];
        }
        return [];
    }
}

// TODO: Add analyzer to ensure the CompareWith type in the attribute matches the property type
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class GreaterThanAttribute : ValidatorAttribute
{

    // This is a positional argument
    public GreaterThanAttribute(object compareWith, string? customMessage = null)
        : base(nameof(GreaterThanValidator<>))
    {
        CompareWith = compareWith;
    }

    public object CompareWith { get; }
}
