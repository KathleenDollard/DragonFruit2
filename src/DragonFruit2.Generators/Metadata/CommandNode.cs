namespace DragonFruit2.Generators.Metadata;

public record class CommandNode
{
    public required CommandInfo CommandInfo { get; init; }
    public CommandNode? RootCommandNode { get; init; }
    public string RootCommandName
    { get
        {
            var rootCommandNode = RootCommandNode switch
            {
                null => this,
                _ => RootCommandNode
            };
            return rootCommandNode.CommandInfo.Name;
        }
    }
    public CommandNode? Parent { get; internal set; }
    public List<CommandNode> SubCommands
    {
        get
        {
            field ??= [];
            return field;
        }
    }
    public IEnumerable<PropInfo> SelfAndAncestorPropInfos
    {
        get
        {
            return CommandInfo.PropInfos.Concat(AncestorPropInfos);
        }
    }
    public IEnumerable<PropInfo> AncestorPropInfos
    {
        get
        {
            if (Parent is not null)
            {
                foreach (var parentProp in Parent.SelfAndAncestorPropInfos)
                {
                    yield return parentProp;
                }
            }
        }
    }
}
