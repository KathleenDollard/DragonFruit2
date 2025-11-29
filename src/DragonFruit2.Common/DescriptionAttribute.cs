
namespace DragonFruit2;

/// <summary>
/// Provides a textual description for a CLI element (type, field or property).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class DescriptionAttribute : Attribute
{
    /// <summary>
    /// The description text. Never null.
    /// </summary>
    public string Description { get; }

    public DescriptionAttribute() : this(string.Empty) { }

    public DescriptionAttribute(string description) => Description = description ?? string.Empty;
}