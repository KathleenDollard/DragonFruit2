using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DragonFruit2.GeneratorSupport;

/// <summary>
/// This information can be used in a generator to create classes 
/// that support System.DragonFruit.ParseArgs.
/// </summary>
public record class CommandInfo
{
    // TODO: Make these required and use init scope. That will remove warning, but needs some downlevel magic
    public required string Name { get; init; }
    public string? NamespaceName { get; init; }

    public string? Description { get; init; }

    public List<PropInfo> Arguments => field ??= [];

    public List<PropInfo> Options => field ??= [];

    public List<CommandInfo> SubCommands => field ??= [];
}

