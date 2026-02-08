namespace DragonFruit2.Validators;

public static class ValidatorExtensions
{
    extension<TValue>(MemberDataDefinition<TValue> memberDefinition)
        where TValue : IComparable<TValue>
    {
        public void ValidateGreaterThan(TValue compareWithValue)
        {
            memberDefinition.RegisterValidator(new GreaterThanValidator<TValue>(memberDefinition.DefinitionName, compareWithValue));
        }
    }
}
