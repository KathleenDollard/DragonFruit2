using DragonFruit2.GeneratorSupport;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace DragonFruit2.Generators;

public class DragonFruit2Builder : GeneratorBuilder<CommandInfo>
{
    /// <summary>
    /// Determines whether the specified syntax node represents an invocation of the generic method 'ParseArgs' with
    /// exactly one type argument.
    /// </summary>
    /// <param name="node">The syntax node to evaluate.</param>
    /// <returns>true if the node is an invocation of 'ParseArgs' with a single type argument; otherwise, false.</returns>
    public override bool InitialFilter(SyntaxNode node)
        => (node is InvocationExpressionSyntax inv) &&
        inv.Expression switch
        {
            MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                => gns.Identifier.ValueText == "ParseArgs" && gns.TypeArgumentList.Arguments.Count == 1,
            GenericNameSyntax gns2
                => gns2.Identifier.ValueText == "ParseArgs" && gns2.TypeArgumentList.Arguments.Count == 1,
            _ => false,
        };

    public override CommandInfo? Transform(GeneratorSyntaxContext context)
            // We only get here for ParseArg invocations with a single generic type argument
            => context.Node switch
            {
                InvocationExpressionSyntax invocationSyntax
                       => GetRootCommandInfoFromInvocation(invocationSyntax, context.SemanticModel),
                _ => null,
            };

    public override void OutputSource(SourceProductionContext context, IEnumerable<CommandInfo> items)
    {
        throw new System.NotImplementedException();
    }


    private static CommandInfo? GetRootCommandInfoFromInvocation(InvocationExpressionSyntax invocationSyntax, SemanticModel semanticModel)
    {
        var rootArgTypeArgSymbol = GetArgTypeSymbol(invocationSyntax, semanticModel);
        if (rootArgTypeArgSymbol is null)
            return null; // This occurs when the root arg type does not yet exist
        var rootCommandInfo = CreateCommandInfo(rootArgTypeArgSymbol, semanticModel);
        return rootCommandInfo;
    }

    private static CommandInfo CreateCommandInfo(INamedTypeSymbol typeSymbol, SemanticModel semanticModel)
    {

        var commandInfo = CommandInfoHelpers.CreateCommandInfo(typeSymbol);

        var props = typeSymbol.GetMembers()
                              .OfType<IPropertySymbol>()
                              .Where(p => !p.IsStatic)
                              .Select(p => CommandInfoHelpers.CreatePropInfo(p))
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
            var childCommandInfo = CreateCommandInfo(derivedType, semanticModel);
            commandInfo.SubCommands.Add(childCommandInfo);
        }

        return commandInfo;
    }

    private static IEnumerable<INamedTypeSymbol> GetChildTypes(INamedTypeSymbol typeSymbol)
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
                    derivedTypes.Add(namedTypeSymbol.BaseType);
                    break;
                }
            }
        }
        return derivedTypes;
    }

    private static INamedTypeSymbol? GetArgTypeSymbol(InvocationExpressionSyntax? invocation, SemanticModel semanticModel)
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



}
