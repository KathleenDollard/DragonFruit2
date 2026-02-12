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

[ValidatorAttributeInfo(typeof(MaxLengthValidator))]
public sealed class MaxLengthAttribute : MemberValidatorAttribute
{

    public MaxLengthAttribute(object maxLengthValue)
    {
        MaxLengthValue = maxLengthValue;
    }

    public object MaxLengthValue { get; }
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
