namespace DragonFruit2.Defaults
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DefaultBaseAttribute: Attribute
    {
        public DefaultBaseAttribute(Type validatorType)
        {
            DefaultType = validatorType;
        }

        public Type DefaultType { get; }
    }
}