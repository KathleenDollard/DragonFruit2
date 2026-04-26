using DragonFruit2.Generators.Metadata;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Generators;

public static class DragonFruitExtensions
{
    extension(CommandInfo commandInfo)
    {
        [NotNullIfNotNull("Name")]
        public string? FullName
            => (commandInfo.NamespaceName, commandInfo.Name).FullName;
    }
    extension(CommandNode commandNode)
    {
        [NotNullIfNotNull("Name")]
        public string? FullName
            => commandNode.CommandInfo.FullName;
    }

    extension((string? NamespaceName, string? Name) tuple)
    {
        [NotNullIfNotNull("Name")]
        public string? FullName
        => string.IsNullOrWhiteSpace(tuple.NamespaceName)
             ? tuple.Name
             : $"{tuple.NamespaceName}.{tuple.Name}";
    }
}