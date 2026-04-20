namespace DragonFruit2.Generators.Metadata;

/// <summary>
/// This information can be used in a generator to create classes 
/// that support System.DragonFruit.ParseArgs.
/// </summary>
public record class CommandInfo
{
    private string? simpleName;

    public required string Name { get; init; }
    public string? NamespaceName { get; init; }
    public string FullName
        => NamespaceName is null
            ? Name
            : $"{NamespaceName}.{Name}";
    //public string? CliNamespaceName { get; init; }
    public required string Accessibility {  get; init; }
    public required string? BaseTypeName { get; init; }
    public required string? BaseTypeNamespace { get; init; }
    //public required string? RootName { get; init; }

    public string? SimpleName
    {
        get => simpleName switch
        {
            null => $"{ToSimpleName(Name)}",
            _ => simpleName
        };
        init => simpleName = value;
    }

    private string ToSimpleName(string name)
    {
        name = Name.EndsWith("Args")
               ? Name.Substring(0, Name.Length - 4)
               : Name;
        return name.ToKebabCase();
    }
    //public CommandInfo? ParentCommandInfo { get; set; } = null;

    public List<PropInfo> Arguments => field ??= [];

    public List<PropInfo> Options => field ??= [];

    public IEnumerable<PropInfo> PropInfos => Options.Concat(Arguments);

}

