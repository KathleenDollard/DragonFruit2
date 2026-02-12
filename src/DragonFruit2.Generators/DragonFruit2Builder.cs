using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace DragonFruit2.Generators;

public class DragonFruit2Builder : GeneratorBuilder<CommandInfo>
{
    private enum CliSymbolType
    {
        Option,
        Argument,
        SubCommand
    }

    /// <summary>
    /// Determines whether the specified syntax node represents an invocation of the generic method 'ParseArgs' with
    /// exactly one type argument.
    /// </summary>
    /// <param name="node">The syntax node to evaluate.</param>
    /// <returns>true if the node is an invocation of 'ParseArgs' with a single type argument; otherwise, false.</returns>
    public override bool InitialFilter(SyntaxNode node)
    {
        try
        {
            return (node is InvocationExpressionSyntax inv) &&
                inv.Expression switch
                {
                    MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                        => IsMethodNameOfInterest(gns.Identifier.ValueText) && gns.TypeArgumentList.Arguments.Count == 1,
                    GenericNameSyntax gns2
                        => IsMethodNameOfInterest(gns2.Identifier.ValueText) && gns2.TypeArgumentList.Arguments.Count == 1,
                    _ => false,
                };
        }
        catch
        {
            throw;
        }
    }

    internal static bool IsMethodNameOfInterest(string valueText)
    {
        return valueText == "ParseArgs" || valueText == "TryParseArgs" || valueText == "TryExecute";
    }

    public override CommandInfo? Transform(GeneratorSyntaxContext context)
    {
        try
        {
            // We only get here for ParseArg invocations with a single generic type argument
            return context.Node switch
            {
                InvocationExpressionSyntax invocationSyntax
                       => GetRootCommandInfoFromInvocation(invocationSyntax, context.SemanticModel),
                _ => null,
            };
        }
        catch
        {
            throw;
        }
    }

    public override void OutputSource(SourceProductionContext context, CommandInfo commandInfo)
    {
        try
        {
            context.AddSource(commandInfo.Name, GetSourceForCommandInfo(commandInfo));
        }
        catch
        {
            throw;
        }
    }

    public void OutputCliSource(SourceProductionContext context, IEnumerable<CommandInfo> commandInfos)
    {
        try
        {
            context.AddSource("Cli", GetSourceForCli(commandInfos));
        }
        catch
        {
            throw;
        }
    }

    private string GetSourceForCli(IEnumerable<CommandInfo> commandInfos)
    {
        return OutputCli.GetSource(commandInfos);
    }

    internal static CommandInfo? GetRootCommandInfoFromInvocation(InvocationExpressionSyntax invocationSyntax,
                                                                  SemanticModel semanticModel)
    {
        var cliNamespaceName = invocationSyntax.GetNamespace();

        var rootArgsTypeArgSymbol = GetArgsTypeSymbol(invocationSyntax, semanticModel);
        if (rootArgsTypeArgSymbol is null)
            return null; // This occurs when the root arg type does not yet exist
        var rootCommandInfo = CreateCommandInfo(rootArgsTypeArgSymbol,
                                                rootArgsTypeArgSymbol.Name,
                                                cliNamespaceName,
                                                semanticModel);
        return rootCommandInfo;
    }

    private static CommandInfo CreateCommandInfo(INamedTypeSymbol typeSymbol,
                                                 string? rootName,
                                                 string? cliNamespaceName,
                                                 SemanticModel semanticModel)
    {
        var commandInfo = CommandInfoHelpers.CreateCommandInfo(typeSymbol, rootName, cliNamespaceName);
        // future: Check perf here (semanticModel is captured, etc)
        var props = typeSymbol.GetMembers()
                              .OfType<IPropertySymbol>()
                              .Where(p => !p.IsStatic)
                              .Select(p => PropInfoHelpers.CreatePropInfo(p, semanticModel))
                              .ToList();

        // Split into argument list and options
        var argList = props.Where(p => p.IsArgument).OrderBy(p => p.Position).ToList();
        var optList = props.Where(p => !p.IsArgument).ToList();
        commandInfo.Arguments.AddRange(argList);
        commandInfo.Options.AddRange(optList);

        var compilation = semanticModel.Compilation;

        var derivedTypes = GetChildTypes(typeSymbol);
        foreach (var derivedType in derivedTypes)
        {
            var childCommandInfo = CreateCommandInfo(derivedType, rootName, cliNamespaceName, semanticModel);
            commandInfo.SubCommands.Add(childCommandInfo);
        }

        return commandInfo;
    }

    internal static IEnumerable<INamedTypeSymbol> GetChildTypes(INamedTypeSymbol typeSymbol)
    {
        var derivedTypes = new List<INamedTypeSymbol>();
        var nspace = typeSymbol.ContainingNamespace;
        foreach (var member in nspace.GetMembers())
        {
            if (member is INamedTypeSymbol namedTypeSymbol)
            {
                // Check if this type derives from the given typeSymbol
                if (SymbolEqualityComparer.Default.Equals(namedTypeSymbol.BaseType, typeSymbol))
                {
                    // This namedTypeSymbol derives from typeSymbol
                    derivedTypes.Add(namedTypeSymbol);
                    continue;
                }
            }
        }
        return derivedTypes;
    }

    private static INamedTypeSymbol? GetArgsTypeSymbol(InvocationExpressionSyntax? invocation, SemanticModel semanticModel)
    {
        if (invocation is null) return null;

        GenericNameSyntax? genericName = invocation.Expression switch
        {
            MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                => gns,
            GenericNameSyntax gns
                => gns,
            _ => null,
        };

        if (genericName is null) return null;

        var typeArgSyntax = genericName.TypeArgumentList.Arguments[0];
        return semanticModel.GetSymbolInfo(typeArgSyntax).Symbol as INamedTypeSymbol;
    }

    internal static string GetSourceForCommandInfo(CommandInfo commandInfo)
    {
        return OutputPartialArgs.GetSourcePartialArgs(commandInfo);
    }

    internal IEnumerable<CommandInfo> GetSelfAndDescendants(CommandInfo commandInfo)
    {
        List<CommandInfo> ret = [];
        ret.Add(commandInfo);
        foreach (var sub in commandInfo.SubCommands)
        {
            ret.AddRange(GetSelfAndDescendants(sub));
        }
        return ret;
    }

    internal ImmutableArray<CommandInfo> BindParentsAndRemoveDuplicates(ImmutableArray<CommandInfo> commandInfos)
    {
        try
        {
            commandInfos = commandInfos.Distinct(new CommandInfoEqualityComparer()).ToImmutableArray();
            foreach (var commandInfo in commandInfos)
            {
                BindParentsRecursive(commandInfo);
            }
            return commandInfos;

            static void BindParentsRecursive(CommandInfo commandInfo)
            {
                foreach (var sub in commandInfo.SubCommands)
                {
                    sub.ParentCommandInfo = commandInfo;
                    BindParentsRecursive(sub);
                }
            }
        }
        catch
        {
            throw;
        }
    }
}
