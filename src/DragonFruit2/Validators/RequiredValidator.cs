namespace DragonFruit2.Validators;

public class RequiredValidator<TValue> : Validator<TValue>
{
    public RequiredValidator(string valueName, string? customMessage = null)
        : base((int)DiagnosticId.Required, valueName)
    {    }

    public override string Description => $"The value of {ValueName} is required";

    public override IEnumerable<Diagnostic<TValue>> Validate(DataValue<TValue> dataValue)
    {
        if (!dataValue.IsSet)
        {
            var message = $"The value of {ValueName} is required and it was not entered.";
            return [new Diagnostic<TValue>(Id, DiagnosticSeverity.Error, ValueName, dataValue.Value, message)];
        }
        return [];
    }
}

[ValidatorAttribute(typeof(RequiredValidator<>))]
public sealed class RequiredAttribute : ValidatorBaseAttribute
{}

public static class RequiredValidatorExtensions
{
    extension<TValue>(MemberDataDefinition<TValue> memberDefinition)
    {
        public void Required()
        {
            memberDefinition.RegisterValidator(new RequiredValidator<TValue>(memberDefinition.DefinitionName));
        }
    }
}
