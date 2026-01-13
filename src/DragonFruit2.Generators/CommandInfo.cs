namespace DragonFruit2.GeneratorSupport;

/// <summary>
/// This information can be used in a generator to create classes 
/// that support System.DragonFruit.ParseArgs.
/// </summary>
public record class CommandInfo
{
    private string? cliName;

    // TODO: Make these required and use init scope. That will remove warning, but needs some downlevel magic
    public required string Name { get; init; }
    public string? NamespaceName { get; init; }
    public string? CliNamespaceName { get; init; }
    public required string ArgsAccessibility {  get; init; }
    public string? BaseName { get; init; }
    public string? RootName { get; init; }
    public string? CliName
    {
        get => cliName switch
        {
            null => $"{Name.ToKebabCase()}",
            _ => cliName
        };
        init => cliName = value;
    }

    public CommandInfo? ParentCommandInfo { get; set; } = null;

    public string? Description { get; init; } = null;
    public bool IsStruct { get; init; }

    public List<PropInfo> Arguments => field ??= [];

    public List<PropInfo> Options => field ??= [];

    public IEnumerable<PropInfo> PropInfos => Options.Concat(Arguments);
    public IEnumerable<PropInfo> SelfAndAncestorPropInfos
    {
        get
        {
            return PropInfos.Concat(AncestorPropInfos);
        }
    }
    public IEnumerable<PropInfo> AncestorPropInfos
    {
        get
        {
            if (ParentCommandInfo is not null)
            {
                foreach (var parentProp in ParentCommandInfo.SelfAndAncestorPropInfos)
                {
                    yield return parentProp;
                }
            }
        }
    }

    public List<CommandInfo> SubCommands => field ??= [];
}

