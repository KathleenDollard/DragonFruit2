namespace DragonFruit2.Validators
{
    public class ValidatorAttributeInfo : Attribute
    {
        public ValidatorAttributeInfo(Type validatorType)
        {
            ValidatorType = validatorType;
        }

        public Type  ValidatorType { get; }
    }
}