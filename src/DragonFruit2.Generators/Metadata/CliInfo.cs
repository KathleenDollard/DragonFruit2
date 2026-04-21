using System.Xml.Linq;

namespace DragonFruit2.Generators.Metadata;

/// <summary>
/// Info that is per command tree
/// </summary>
public record class CliInfo
{
    // Roslyn treats the global namespace as a null, and this class does the same

    public required string? RootTypeNamespace {  get; init; }
    public required string RootCommandName { get; init; }

    public required string? EntryPointNamespace { get; init; }

    // In general, the Info classes are immutable, however, allowing the RootCommand and Command  namespaces
    // to be collected and modified during tree creation sets the precedent that additional metadata
    // classes not be created for new concerns.
    // Null is not allowed, there is no need for a using for the global namespace, it's kind of the point of it.
    public List<string> CommandNamespaces { get; } = [];
    public CommandNode? RootCommandNode { get; internal set; }   

}
