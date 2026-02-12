namespace DragonFruit2.Validators;


public sealed class RequiredAttribute : MemberValidatorAttribute
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
    public RequiredAttribute(bool isRquired = true)
        : base(typeof(GreaterThanValidator<>))
    { }

}
