namespace DragonFruit2.Validators;

public class LessThanValidator<TValue> : Validator<TValue>
    where TValue : IComparable<TValue>
{
    public LessThanValidator(string valueName, TValue compareWithValue)
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
        if (dataValue.Value!.CompareTo(CompareWithValue) >= 0)
        {
            var message = $"The value of {ValueName} must be greater than {CompareWithValue}, and {dataValue.Value} is not.";
            return [new Diagnostic<TValue>(Id, DiagnosticSeverity.Error, ValueName, dataValue.Value, message)];
        }
        return [];
    }
}

// TODO: Add analyzer to ensure the CompareWith type in the attribute matches the property type
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class LessThanAttribute : ValidatorAttribute
{

    // This is a positional argument
    public LessThanAttribute(object compareWith)
        : base(typeof(GreaterThanValidator<>))
    {
        CompareWith = compareWith;
    }

    public object CompareWith { get; }
}

public static class LessThanValidatorExtensions
{
    extension<TValue>(MemberDataDefinition<TValue> memberDefinition)
        where TValue : IComparable<TValue>
    {
        public void ValidateLessThan(TValue compareWithValue)
        {
            memberDefinition.RegisterValidator(new LessThanValidator<TValue>(memberDefinition.DefinitionName, compareWithValue));
        }
    }
}
