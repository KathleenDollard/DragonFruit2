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

    public override IEnumerable<Diagnostic<TValue>> Validate(DataValue<TValue> dataValue)
    {
        if (!typeof(TValue).IsValueType && dataValue.Value == null)
        {
            return Enumerable.Empty<Diagnostic<TValue>>();
        }
        if (dataValue.Value!.CompareTo(CompareWithValue) <= 0)
        {
            var message = $"The value of {ValueName} must be greater than {CompareWithValue}, and {dataValue.Value} is not.";
            return [new Diagnostic<TValue>(Id, DiagnosticSeverity.Error, ValueName, dataValue.Value, message)];
        }
        return [];
    }
}

// TODO: Add analyzers to ensure the CompareWith type in the attribute matches the property type
// // TODO: Add an analyzer that ensures validator constructor parameters appear as properties on attribute
[VaidatorAttributeInfo(nameof(GreaterThanValidatorExtensions.ValidateGreaterThan))]
public sealed class GreaterThanAttribute : ValidatorAttribute
{

    // This is a positional argument
    public GreaterThanAttribute(object compareWith, string? customMessage = null)
        : base(typeof(GreaterThanValidator<>))
    {

        CompareWith = compareWith;
    }

    public object CompareWith { get; }
}

public static class GreaterThanValidatorExtensions
{
    extension<TValue>(MemberDataDefinition<TValue> memberDefinition)
        where TValue : IComparable<TValue>
    {
        public void ValidateGreaterThan(TValue compareWithValue)
        {
            memberDefinition.RegisterValidator(new GreaterThanValidator<TValue>(memberDefinition.DefinitionName, compareWithValue));
        }
    }
}