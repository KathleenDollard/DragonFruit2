// These attributes are deliberately in the same file because the nuances between them
// can be confusing. Please read the summaries as the differences are significant

namespace DragonFruit2.Validators
{
    /// <summary>
    /// This attribute _must_ be present on all attributes used for validation or defaults.
    /// </summary>
    /// <remarks>
    /// While the name looks funny here, this means that in usage it will read `MemberAttribute`,
    /// which seems the clearest name.
    /// <br/>
    /// This attrbute is used by the generator to find the underlying class. This type must exist,
    /// and must have a constructor that matches the attribute constructor in name. The types ofd
    /// each constructor parameter must be castable to the type of the corresponding parameter in 
    /// the underlying validator or default type.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MemberAttributeAttribute : Attribute
    {
        public MemberAttributeAttribute(Type validatorOrDefaultType)
        {
            HelperType = validatorOrDefaultType;
        }

        public Type HelperType { get; }
    }

    /// <summary>
    /// This attribute is a convenience for use validator attributes. Many people write
    /// attributes without considering usage, and this provides a decent default.
    /// </summary>
    /// <remarks>
    /// Whether to allow multiplle is inherited to any derived attributes and means that
    /// the specified attribute can generally appear only once. While several validation
    /// attributes will appear, they are expected to be different - such as a [GreaterThan(0)]
    /// and [LessThan(10)] combination.
    /// </remarks>

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidatorBaseAttribute : Attribute
    {

    }

    /// <summary>
    /// This attribute is a convenience for use validator attributes. Many people write
    /// attributes without considering usage, and this provides a decent default.
    /// </summary>
    /// <remarks>
    /// Whether to allow multiplle is inherited to any derived attributes and means that
    /// the specified attribute can generally appear only once. While several validation
    /// attributes will appear, they are expected to be different - such as a [GreaterThan(0)]
    /// and [LessThan(10)] combination.
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DefaultBaseAttribute : Attribute
    {

    }
}