namespace DragonFruit2.Generators.Metadata;

public record class CommandNode
{
    public string Name => CommandInfo.Name;
    public string? ParentCommandFullName => ParentCommand?.CommandInfo.FullName;
    public string RootCommandFullName
    {
        get
        {
            var rootCommandNode = RootCommandNode switch
            {
                null => this,
                _ => RootCommandNode
            };
            return rootCommandNode.CommandInfo.FullName;
        }
    }

    public List<CommandNode> SubCommands => field ??= [];


    public CommandNode? RootCommandNode { get; init; }
    public required CommandInfo CommandInfo { get; init; }
    public CommandNode? ParentCommand { get; internal set; }
    public IEnumerable<PropInfo> GetSelfAndAncestorPropInfos()
        => CommandInfo.GetOptionsAndArguments().Concat(GetAncestorPropInfos());

    public IEnumerable<PropInfo> GetAncestorPropInfos()
    {
        if (ParentCommand is not null)
        {
            foreach (var parentProp in ParentCommand.GetSelfAndAncestorPropInfos())
            {
                yield return parentProp;
            }
        }
    }
}
