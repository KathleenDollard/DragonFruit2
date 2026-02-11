namespace DragonFruit2;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public abstract class ValidatorAttribute : Attribute
{

    public ValidatorAttribute(Type validatorType)
    {
        ValidatorType = validatorType;
    }

    public Type ValidatorType { get; }

    public void SetValidator()
}