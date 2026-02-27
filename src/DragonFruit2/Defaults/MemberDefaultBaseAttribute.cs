namespace DragonFruit2.Defaults
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class MemberDefaultBaseAttribute: Attribute
    {
        public MemberDefaultBaseAttribute(Type validatorType)
        {
            DefaultType = validatorType;
        }

        public Type DefaultType { get; }
    }
}