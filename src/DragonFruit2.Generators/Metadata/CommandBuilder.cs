using DragonFruit2.Generators.CodeOutput;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace DragonFruit2.Generators.Metadata;

public class CommandBuilder
{
    internal static CommandInfo? GetCommandInfo(ClassDeclarationSyntax classDeclarationSyntax,
                                                SemanticModel semanticModel)
    {
        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
        var classTypeSymbol = classSymbol as INamedTypeSymbol;
        if (classTypeSymbol is null) return null;

        var commandInfo = new CommandInfo
        {
            // TODO: Add description from attribute if present or XML docs
            Name = classTypeSymbol.Name,
            NamespaceName = classTypeSymbol.GetNamespace(),
            Accessibility = classTypeSymbol.DeclaredAccessibility.ToCSharpString(),
            BaseTypeName = classTypeSymbol.BaseType?.Name,
            BaseTypeNamespace = classTypeSymbol.BaseType?.GetNamespace(),
        };
        // future: Check perf here (semanticModel is captured, etc)
        var props = classTypeSymbol.GetMembers()
                              .OfType<IPropertySymbol>()
                              .Where(p => !p.IsStatic)
                              .Select(p => PropInfoHelpers.CreatePropInfo(p, semanticModel))
                              .ToList();

        // Split into argument list and options
        var argList = props.Where(p => p.IsArgument).OrderBy(p => p.Position).ToList();
        var optList = props.Where(p => !p.IsArgument).ToList();
        commandInfo.Arguments.AddRange(argList);
        commandInfo.Options.AddRange(optList);


        return commandInfo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// A project may have more than one tree.
    /// </remarks>
    /// <param name="commandInfos"></param>
    /// <returns>A collection of CommandNode objects that correspond to all of the command roots in the compilation</returns>
    internal static IEnumerable<CommandNode> BuildCommandNodes(IEnumerable<CommandInfo> commandInfos, CancellationToken ctx)
    {
        var commandNodes = commandInfos
                .Select(info => new CommandNode { CommandInfo = info }).ToList();

        var lookup = commandNodes.ToLookup(node => node.CommandInfo.BaseTypeFullName);

        foreach (var node in commandNodes)
        {
            IEnumerable<CommandNode> subCommands = [.. lookup[node.CommandInfo.FullName]];
            if (!subCommands.Any() || node.SubCommands is null) continue;

            node.SubCommands.AddRange(subCommands);
            foreach (var subCommand in subCommands)
            {
                subCommand.ParentCommandNode = node;
            }
        }

        foreach (var commandNode in commandNodes)
        {
            commandNode.SetRootCommandNode();
        }

        return commandNodes;
    }

    internal static IEnumerable<CliInfoGroup> GetCliInfoGroups(IEnumerable<CliInfo> cliInfos, CancellationToken token)
    {
        var grouped = cliInfos.GroupBy(cliInfo => cliInfo.EntryPointNamespace);

        return grouped.Select(g => new CliInfoGroup(g)).ToList();
    }

    internal static IEnumerable<CommandNode> FlattenHierarchy(IEnumerable<CommandNode> commandNodes, CancellationToken ctx)
    {
        List<CommandNode> list = [];
        foreach (var node in commandNodes)
        {
            FlattenHierarchy(node, list);
        }
        return list;

        static IEnumerable<CommandNode> FlattenHierarchy(CommandNode commandNode, List<CommandNode> list)
        {
            list.Add(commandNode);
            foreach (var sub in commandNode.SubCommands)
            {
                list.AddRange(FlattenHierarchy(sub, list));
            }
            return list;
        }
    }

    internal static ImmutableArray<IGrouping<string?, CliInfo>> SpreadCliInfos(IEnumerable<IGrouping<string?, CliInfo>> cliInfos, CancellationToken token)
    {
        return cliInfos.ToImmutableArray();
    }


    //internal static IEnumerable<INamedTypeSymbol> GetChildTypes(INamedTypeSymbol typeSymbol)
    //{
    //    var derivedTypes = new List<INamedTypeSymbol>();
    //    var nspace = typeSymbol.ContainingNamespace;
    //    foreach (var member in nspace.GetMembers())
    //    {
    //        if (member is INamedTypeSymbol namedTypeSymbol)
    //        {
    //            // Check if this type derives from the given typeSymbol
    //            if (SymbolEqualityComparer.Default.Equals(namedTypeSymbol.BaseType, typeSymbol))
    //            {
    //                // This namedTypeSymbol derives from typeSymbol
    //                derivedTypes.Add(namedTypeSymbol);
    //                continue;
    //            }
    //        }
    //    }
    //    return derivedTypes;
    //}

    //internal static IEnumerable<CommandInfo> GetSelfAndDescendants(CommandInfo commandInfo)
    //{
    //    List<CommandInfo> ret = [];
    //    ret.Add(commandInfo);
    //    foreach (var sub in commandInfo.SubCommands)
    //    {
    //        ret.AddRange(GetSelfAndDescendants(sub));
    //    }
    //    return ret;
    //}

    //internal static ImmutableArray<CommandInfo> BindParentsAndRemoveDuplicates(ImmutableArray<CommandInfo> commandInfos)
    //{
    //    try
    //    {
    //        commandInfos = commandInfos.Distinct(new CommandInfoEqualityComparer()).ToImmutableArray();
    //        foreach (var commandInfo in commandInfos)
    //        {
    //            BindParentsRecursive(commandInfo);
    //        }
    //        return commandInfos;

    //        static void BindParentsRecursive(CommandInfo commandInfo)
    //        {
    //            foreach (var sub in commandInfo.SubCommands)
    //            {
    //                sub.ParentCommandInfo = commandInfo;
    //                BindParentsRecursive(sub);
    //            }
    //        }
    //    }
    //    catch
    //    {
    //        throw;
    //    }
    //}


}
