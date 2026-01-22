namespace DragonFruit2.Validators;

public class MaxLengthValidator : Validator<string>
{
    public MaxLengthValidator(string valueName, int maxLengthValue)
        : base((int)ValidationId.MaxLength, valueName)
    {
        maxLengthValue = MaxLengthValue;
    }

    public override string Description => $"The string {ValueName} must be longer than {MaxLengthValue}";
    public int MaxLengthValue { get; }

    public override IEnumerable<ValidationFailure<string>> Validate(string value)
    {
        if (value.Length < MaxLengthValue)
        {
            var message = $"The value of {ValueName} must be greater than {MaxLengthValue}, and {value} is not.";
            return [new ValidationFailure<string>(Id, message, ValueName, DiagnosticSeverity.Error, value)];
        }
        return [];
    }
}

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class MaxLengthAttribute : ValidatorAttribute
{

    // This is a positional argument
    public MaxLengthAttribute(object compareWith)
        : base(nameof(MaxLengthValidator))
    {
        CompareWith = compareWith;
    }

    public object CompareWith { get; }
}
