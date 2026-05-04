using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DragonFruit2.Generators.Metadata;

internal class CliBuilder
{
    internal static CliInfo? GetCliInfo(InvocationExpressionSyntax invocationSyntax, SemanticModel semanticModel)
    {
        if (invocationSyntax is null) return null;

        var cliNamespaceName = invocationSyntax.GetNamespace();

        var genericNameSyntax = invocationSyntax.GetGenericNameSyntax();
        if (genericNameSyntax is null) return null;

        var rootTypeSymbol = semanticModel.GetTypeArgumentSymbol(genericNameSyntax);
        if (rootTypeSymbol is null) return null; // This occurs when the root arg type does not yet exist and we don't want to add to the existing error

        return new CliInfo
        {
            RootCommandName = rootTypeSymbol.Name,
            RootTypeNamespace = rootTypeSymbol.GetNamespace() ?? "",
            EntryPointNamespace = cliNamespaceName,

        };
    }


    internal static bool IsMethodNameOfInterest(string name)
    {
        string[] targetNames = ["TryParse", "TryParse", "TryExecute"];
        return targetNames.Contains(name);
    }
}