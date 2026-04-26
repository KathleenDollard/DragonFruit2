namespace DragonFruit2.Generators.Metadata;

public record class CommandNode
{
    private CommandNode? _rootCommandNode = null;

    public string Name => CommandInfo.Name;

    public List<CommandNode> SubCommands => field ??= [];

    // This is not required because it is set after the tree is built
    public CommandNode? RootCommandNode => _rootCommandNode;

    public required CommandInfo CommandInfo { get; init; }
    public CommandNode? ParentCommandNode { get; internal set; }
    public IEnumerable<PropInfo> GetSelfAndAncestorPropInfos()
        => CommandInfo.GetOptionsAndArguments().Concat(GetAncestorPropInfos());

    public IEnumerable<PropInfo> GetAncestorPropInfos()
    {
        if (ParentCommandNode is not null)
        {
            foreach (var parentProp in ParentCommandNode.GetSelfAndAncestorPropInfos())
            {
                yield return parentProp;
            }
        }
    }

    internal void SetRootCommandNode()
        => _rootCommandNode = GetAncestorsAndSelf().Last();

    public IEnumerable<CommandNode> GetAncestorsAndSelf()
    { 
        yield return this;
        var current = ParentCommandNode;
        while (current != null)
        {
            yield return current;
            current = current.ParentCommandNode;
        }
    }
}
