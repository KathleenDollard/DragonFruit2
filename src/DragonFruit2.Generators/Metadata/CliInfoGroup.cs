using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Generators.Metadata;

/// <summary>
/// CliInfo grouped by EntryPointNamespace; used to generate one file per entry point namespace
/// </summary>
/// <remarks>
/// This class avoids exposing that we are using a LINQ group to the outputting code to 
/// allow testing of the groups and improve isolation
/// </remarks>
public record class CliInfoGroup
{
    [SetsRequiredMembers]
    internal CliInfoGroup(System.Linq.IGrouping<string?, CliInfo> group)
    {
        CliInfos = group.ToList();
        EntryPointNamespace = group.Key;
    }

    public required string? EntryPointNamespace { get; init; }

    public IEnumerable<CliInfo> CliInfos { get; } = [];

}