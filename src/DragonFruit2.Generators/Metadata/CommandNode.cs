namespace DragonFruit2.Generators.Metadata;

public record class CommandNode
{

    public string Name => CommandInfo.Name;

    public List<CommandNode> SubCommands => field ??= [];

    // This is not required because it is set after the tree is built
    public CommandNode? RootCommandNode { get; private set; }

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
        => RootCommandNode = GetAncestorsAndSelf().Last();

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

    public virtual bool Equals(CommandNode? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // Compare the properties of CommandNode
        return CommandInfo == other.CommandInfo
            && ParentCommandNode?.FullName == other.ParentCommandNode?.FullName
            && RootCommandNode?.FullName == other.RootCommandNode?.FullName
            && SubCommands.SequenceEqual(other.SubCommands);
    }

    public override int GetHashCode()
    {
        var hashCode = new System.HashCode();
        hashCode.Add(CommandInfo);
        hashCode.Add(ParentCommandNode?.FullName);
        hashCode.Add(RootCommandNode?.FullName);
        foreach (var subCommand in SubCommands)
        {
            hashCode.Add(subCommand);
        }
        return hashCode.ToHashCode();
    }
}
