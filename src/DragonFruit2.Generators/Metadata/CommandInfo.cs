namespace DragonFruit2.Generators.Metadata;

/// <summary>
/// This information can be used in a generator to create classes 
/// that support System.DragonFruit.ParseArgs.
/// </summary>
public record class CommandInfo
{
    public required string Name { get; init; }
    public string? NamespaceName { get; init; }
    public required string Accessibility {  get; init; }

    // The base info is used to create the CommandNode tree
    public required string? BaseTypeName { get; init; }
    public required string? BaseTypeNamespace { get; init; }
    public string? BaseTypeFullName => (BaseTypeNamespace, BaseTypeName).FullName;
   
    public string? SimpleName
    {
        get => field switch
        {
            null => $"{ToSimpleName(Name)}",
            _ => field
        };
        init;
    }

    private string ToSimpleName(string name)
    {
        name = Name.EndsWith("Args")
               ? Name.Substring(0, Name.Length - 4)
               : Name;
        return name.ToKebabCase();
    }

    public List<PropInfo> Arguments => field ??= [];

    public List<PropInfo> Options => field ??= [];

    public IEnumerable<PropInfo> GetOptionsAndArguments() => Options.Concat(Arguments);

}

