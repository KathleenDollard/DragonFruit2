namespace DragonFruit2.Validators;

public class ConstantValidator<TValue> : Validator<TValue>
{
    public ConstantValidator(string valueName, TValue compareWithValue, string? customMessage = null)
        : base((int)DiagnosticId.GreaterThan, valueName)
    {
        CompareWithValue = compareWithValue;
    }

    public override string Description => $"The value of {ValueName} must be greater than {CompareWithValue}";
    public TValue CompareWithValue { get; }

    public override IEnumerable<Diagnostic<TValue>> Validate(DataValue<TValue> dataValue)
    {

        return [];
    }
}

[ValidatorAttribute(typeof(ConstantValidator<>))]
public class ConstantValidatorAttribute : ValidatorBaseAttribute
{
    public ConstantValidatorAttribute(object compareWithValue, string? customMessage = null)
    {
        CompareWith = compareWithValue;
    }
    public object CompareWith { get; set; }
}







