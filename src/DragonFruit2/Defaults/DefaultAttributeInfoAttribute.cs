namespace DragonFruit2.Defaults;

public class DefaultAttributeInfoAttribute : Attribute
{
    public DefaultAttributeInfoAttribute(Type defaultType)
    {
        DefaultType = defaultType;
    }

    public Type DefaultType { get; }
}