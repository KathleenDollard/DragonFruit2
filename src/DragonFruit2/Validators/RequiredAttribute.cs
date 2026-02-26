namespace DragonFruit2.Validators;

public class RequiredValidator<TValue> : Validator<TValue>
{
    public RequiredValidator(string valueName, bool isRequired, string? customMessage = null)
        : base((int)DiagnosticId.Required, valueName)
    {
       IsRequired = isRequired;
    }

    public override string Description => $"The value of {ValueName} is required";
    public bool IsRequired { get; }

    public override IEnumerable<Diagnostic<TValue>> Validate(DataValue<TValue> dataValue)
    {
        if (!typeof(TValue).IsValueType && dataValue.Value == null)
        {
            return Enumerable.Empty<Diagnostic<TValue>>();
        }
        if (IsRequired && !dataValue.IsSet)
        {
            var message = $"The value of {ValueName} is required and it was present.";
            return [new Diagnostic<TValue>(Id, DiagnosticSeverity.Error, ValueName, dataValue.Value, message)];
        }
        return [];
    }
}

public sealed class RequiredAttribute : MemberAttributeAttribute
{

    /// <summary>
    /// Declares whether a value is required to create an instance of the class.
    /// If the attribute is not present, default required rules are used based on 
    /// type and nullability, and are generally correct.
    /// </summary>
    /// <remarks>
    /// The checks are required values are done after all defaults are applied and
    /// before the object is created, and thus before validation.
    /// </remarks>
    /// <param name="isRquired"></param>
    public RequiredAttribute(bool isRequired = true)
        : base(typeof(RequiredValidator<>))
    { }

}

public static class RequiredValidatorExtensions
{
    extension<TValue>(MemberDataDefinition<TValue> memberDefinition)
    {
        public void Required(bool isRequired = true)
        {
            memberDefinition.RegisterValidator(new RequiredValidator<TValue>(memberDefinition.DefinitionName, isRequired));
        }
    }
}
