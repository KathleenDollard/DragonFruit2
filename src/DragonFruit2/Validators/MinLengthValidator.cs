namespace DragonFruit2.Validators;

public class MinLengthValidator : Validator<string>
{
    public MinLengthValidator(string valueName, int minLengthValue)
        : base((int)ValidationId.MinLength, valueName)
    {
        minLengthValue = MinLengthValue;
    }

    public override string Description => $"The string {ValueName} must be longer than {MinLengthValue}";
    public int MinLengthValue { get; }

    public override IEnumerable<ValidationFailure<string>> Validate(string value)
    {
        if (value.Length < MinLengthValue)
        {
            var message = $"The value of {ValueName} must be greater than {MinLengthValue}, and {value} is not.";
            return [new ValidationFailure<string>(Id, message, ValueName, DiagnosticSeverity.Error, value)];
        }
        return [];
    }
}

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class MinLengthAttribute : ValidatorAttribute
{

    // This is a positional argument
    public MinLengthAttribute(object compareWith)
        : base(nameof(MinLengthValidator))
    {
        CompareWith = compareWith;
    }

    public object CompareWith { get; }
}
