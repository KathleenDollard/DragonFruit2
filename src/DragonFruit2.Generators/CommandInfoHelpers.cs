using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DragonFruit2.Generators;

public static class CommandInfoHelpers
{
    public static CommandInfo CreateCommandInfo(INamedTypeSymbol typeSymbol,
                                                string? rootName,
                                                string? cliNamespaceName)
    {
        string? baseTypeName = typeSymbol.Name == rootName ? null : typeSymbol.BaseType?.Name;

        return new()
        {
            // TODO: Add description from attribute if present or XML docs
            Name = typeSymbol.Name,
            NamespaceName = typeSymbol.GetNamespace(),
            CliNamespaceName = cliNamespaceName,
            ArgsAccessibility = typeSymbol.DeclaredAccessibility.ToCSharpString(),
            BaseName = baseTypeName,
            RootName = rootName,
        };
    }
}
