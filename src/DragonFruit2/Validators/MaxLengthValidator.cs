namespace DragonFruit2.Validators;

public class MaxLengthValidator : Validator<string>
{
    public MaxLengthValidator(string valueName, int maxLengthValue)
        : base((int)DiagnosticId.MaxLength, valueName)
    {
        maxLengthValue = MaxLengthValue;
    }

    public override string Description => $"The string {ValueName} must be longer than {MaxLengthValue}";
    public int MaxLengthValue { get; }

    public override IEnumerable<Diagnostic<string>> Validate(DataValue<string> dataValue)
    {
        // TODO: Confirm that this is correct null behavior
        if (dataValue.Value is null)
        {
            return Enumerable.Empty<Diagnostic<string>>();
        }
        if (dataValue.Value.Length < MaxLengthValue)
        {
            var message = $"The value of {ValueName} must be greater than {MaxLengthValue}, and {dataValue.Value} is not.";
            return [new Diagnostic<string>(Id, DiagnosticSeverity.Error, ValueName, dataValue.Value, message)];
        }
        return [];
    }
}

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class MaxLengthAttribute : ValidatorAttribute
{

    // This is a positional argument
    public MaxLengthAttribute(object compareWith)
        : base(typeof(MaxLengthValidator))
    {
        CompareWith = compareWith;
    }

    public object CompareWith { get; }
}

public static class MaxLengthValidatorExtensions
{
    extension(MemberDataDefinition<string> memberDefinition)
    {
        public void ValidateMaxLength(int maxLengthValue)
        {
            memberDefinition.RegisterValidator(new MaxLengthValidator(memberDefinition.DefinitionName, maxLengthValue));
        }
    }
}
