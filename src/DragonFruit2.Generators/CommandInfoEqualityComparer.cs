namespace DragonFruit2.Generators;

public class CommandInfoEqualityComparer : IEqualityComparer<CommandInfo>
{
    public bool Equals(CommandInfo x, CommandInfo y)
    {
        return x.NamespaceName == y.NamespaceName
            && x.CliNamespaceName == y.CliNamespaceName
            && x.Name == y.Name 
            && x.SimpleName == y.SimpleName
            && x.RootName == y.RootName;
    }

    public int GetHashCode(CommandInfo x) 
        => (x.NamespaceName, x.CliNamespaceName, x.Name, x.SimpleName, x.RootName).GetHashCode();
}
