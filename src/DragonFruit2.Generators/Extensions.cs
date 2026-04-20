using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace DragonFruit2.Generators;
// TODO: Replace all use of this class with DragonFruit2.Common.StringExtensions
public static class Extensions
{
    extension(ISymbol symbol)
    {
        public T? GetAttributeValue<T>(string namespaceName, string attributeName, string parameterName)
        {
            var attr = symbol.GetAttributes().FirstOrDefault(a =>
                    a.AttributeClass?.Name == attributeName ||
                    a.AttributeClass?.ToDisplayString() == $"{namespaceName}.{parameterName}");

            if (attr == null)
                return default;


            if (attr.ConstructorArguments.Length == 1 && attr.ConstructorArguments[0].Value is T s)
                return s;
            else
            {
                var named = attr.NamedArguments.FirstOrDefault(kv => kv.Key == parameterName);
                if (named.Value.Value is T s2)
                    return s2;
            }
            return default;
        }
    }

        extension(INamedTypeSymbol typeSymbol)
        {
            public string? GetNamespace()
            {
                var ns = typeSymbol.ContainingNamespace;
                if (ns is null || ns.IsGlobalNamespace) return null;
                return ns.ToDisplayString();
            }
        }

    extension(SyntaxNode syntaxNode)
    {
        public string? GetNamespace()
        {
            var namespaceNames = syntaxNode.Ancestors()
                    .OfType<BaseNamespaceDeclarationSyntax>()
                    .Select(s => s.Name.ToString());
            return namespaceNames.Any()
                ? string.Join(".", namespaceNames)
                : null;
        }
    }

    extension(InvocationExpressionSyntax syntaxNode)
    {
        public GenericNameSyntax? GetGenericNameSyntax()
        {
            return syntaxNode.Expression switch
            {
                MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                    => gns,
                GenericNameSyntax gns
                    => gns,
                _ => null,
            };
        }
    }

    extension(SemanticModel semanticModel)
    {
        public INamedTypeSymbol? GetTypeArgumentSymbol(GenericNameSyntax genericName)
        {
            var typeArgSyntax = genericName.TypeArgumentList.Arguments[0];
            return semanticModel.GetSymbolInfo(typeArgSyntax).Symbol as INamedTypeSymbol;
        }
    }
}
