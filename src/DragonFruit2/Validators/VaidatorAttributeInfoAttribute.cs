namespace DragonFruit2.Validators
{
    public class VaidatorAttributeInfoAttribute : Attribute
    {
        public VaidatorAttributeInfoAttribute(string extensionMethodName)
        {
            ExtensionMethodName = extensionMethodName;
        }

        public string ExtensionMethodName { get; }
    }
}