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
        if (group is not null) // a test passes null
        {
            CliInfos = group.ToList();
            EntryPointNamespace = group.Key;
        }
    }

    public required string? EntryPointNamespace { get; init; }

    public List<CliInfo> CliInfos { get; } = [];

    public virtual bool Equals(CliInfoGroup? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // Compare the properties of CliInfoGroup
        return EntryPointNamespace == other.EntryPointNamespace
            && CliInfos.SequenceEqual(other.CliInfos);
    }

    public override int GetHashCode()
    {
        var hashCode = new System.HashCode();
        hashCode.Add(EntryPointNamespace);
        foreach (var cliInfo in CliInfos)
        {
            hashCode.Add(cliInfo);
        }
        return hashCode.ToHashCode();
    }

}