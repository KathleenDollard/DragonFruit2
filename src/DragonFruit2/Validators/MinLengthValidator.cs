namespace DragonFruit2.Validators;

public class MinLengthValidator : Validator<string>
{
    public MinLengthValidator(string valueName, int minLengthValue)
        : base((int)DiagnosticId.MinLength, valueName)
    {
        minLengthValue = MinLengthValue;
    }

    public override string Description => $"The string {ValueName} must be longer than {MinLengthValue}";
    public int MinLengthValue { get; }

    public override IEnumerable<Diagnostic<string>> Validate(DataValue<string> dataValue)
    {
        // TODO: Confirm that this is correct null behavior
        if (dataValue.Value is null)
        {
            return Enumerable.Empty<Diagnostic<string>>();
        }
        if (dataValue.Value.Length < MinLengthValue)
        {
            var message = $"The value of {ValueName} must be greater than {MinLengthValue}, and {dataValue.Value} is not.";
            return [new Diagnostic<string>(Id, DiagnosticSeverity.Error, ValueName, dataValue.Value, message)];
        }
        return [];
    }
}

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class MinLengthAttribute : ValidatorAttribute
{

    // This is a positional argument
    public MinLengthAttribute(object compareWith)
        : base(typeof(MinLengthValidator))
    {
        CompareWith = compareWith;
    }

    public object CompareWith { get; }
}

public static class MinLengthValidatorExtensions
{
    extension(MemberDataDefinition<string> memberDefinition)
    {
        public void ValidateMinimumLength(int maxLengthValue)
        {
            memberDefinition.RegisterValidator(new MinLengthValidator(memberDefinition.DefinitionName, maxLengthValue));
        }
    }
}