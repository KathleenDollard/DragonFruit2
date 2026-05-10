namespace DragonFruit2.Generators.Metadata;

/// <summary>
/// This information can be used in a generator to create classes 
/// that support System.DragonFruit.ParseArgs.
/// </summary>
public record class CommandInfo
{
    public required string Name { get; init; }
    public string? NamespaceName { get; init; }
    public required string Accessibility { get; init; }

    // The base info is used to create the CommandNode tree
    public required string? BaseTypeName { get; init; }
    public required string? BaseTypeNamespace { get; init; }
    public string? BaseTypeFullName => (BaseTypeNamespace, BaseTypeName).FullName;

    public List<PropInfo> Arguments => field ??= [];

    public List<PropInfo> Options => field ??= [];

    public IEnumerable<PropInfo> GetOptionsAndArguments() => Options.Concat(Arguments);

    public virtual bool Equals(CommandInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // Compare the properties of CommandInfo
        return Name == other.Name
            && NamespaceName == other.NamespaceName
            && Accessibility == other.Accessibility
            && BaseTypeName == other.BaseTypeName
            && BaseTypeNamespace == other.BaseTypeNamespace
            && Arguments.SequenceEqual(other.Arguments)
            && Options.SequenceEqual(other.Options);
    }

    public override int GetHashCode()
    {
        var hashCode = new System.HashCode();
        hashCode.Add(Name);
        hashCode.Add(NamespaceName);
        hashCode.Add(Accessibility);
        hashCode.Add(BaseTypeName);
        hashCode.Add(BaseTypeNamespace);
        foreach (var argument in Arguments)
        {
            hashCode.Add(argument);
        }
        foreach (var option in Options)
        {
            hashCode.Add(option);
        }
        return hashCode.ToHashCode();
    }
}

