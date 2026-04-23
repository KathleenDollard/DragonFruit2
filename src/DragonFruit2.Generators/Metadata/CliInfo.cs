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

    //public CommandNode? RootCommandNode { get; internal set; }   

}
