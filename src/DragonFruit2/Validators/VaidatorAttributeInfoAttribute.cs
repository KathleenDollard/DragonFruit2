namespace DragonFruit2.Validators
{
    public class VaidatorAttributeInfoAttribute : Attribute
    {
        public VaidatorAttributeInfoAttribute(Type validatorType)
        {
            ValidatorType = validatorType;
        }

        public Type  ValidatorType { get; }
    }
}