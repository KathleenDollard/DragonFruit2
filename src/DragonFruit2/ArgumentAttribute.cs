namespace DragonFruit2;

/// <summary>
/// Marks a property as a positional CLI argument and optionally specifies its zero-based position.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ArgumentAttribute : Attribute
{
    /// <summary>
    /// Zero-based position of the argument. Use -1 (default) to indicate unspecified.
    /// </summary>
    public int Position { get; }

    public ArgumentAttribute(int position) => Position = position;
}