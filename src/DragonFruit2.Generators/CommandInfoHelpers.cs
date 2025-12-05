using DragonFruit2.GeneratorSupport;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DragonFruit2.Generators;

public static class CommandInfoHelpers
{
    public static CommandInfo CreateCommandInfo(INamedTypeSymbol typeSymbol)
        => new()
        {

            // TODO: Add description from attribute if present or XML docs
            Name = typeSymbol.Name,
            NamespaceName = typeSymbol.GetNamespace(),
            IsStruct = typeSymbol.IsValueType,
        };

    /// <summary>
    /// Build model of properties using semantic and syntactic information available at compile time
    /// </summary>
    /// <param name="propSymbol"></param>
    /// <returns></returns>
    public static PropInfo CreatePropInfo(IPropertySymbol propSymbol)
    {
        var (hasInitializer, initializerText) = GetInitilializerInfo(propSymbol);
        var hasArgumentAttribute = propSymbol.GetAttributes()
            .Any(attr => attr.AttributeClass?.ToDisplayString() == "DragonFruit2.ArgumentAttribute");

        var propInfo = new PropInfo
        {
            Name = propSymbol.Name,
            TypeName = propSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            IsValueType = propSymbol.Type.IsValueType,
            NullableAnnotation = propSymbol.NullableAnnotation,
            HasRequiredModifier = propSymbol.IsRequired,
            Description = propSymbol.GetAttributeValue<string>("DragonFruit2", "DescriptionAttribute", "Description"),
            HasArgumentAttribute = hasArgumentAttribute,
            Position = propSymbol.GetAttributeValue<int>("DragonFruit2", "ArgumentAttribute", "Position"),
            IsArgument = hasArgumentAttribute,
            HasInitializer = hasInitializer,
            InitializerText = initializerText,
        };




        // Decide whether the property should be treated as required for CLI:
        // - explicit 'required' modifier wins
        // - otherwise non-nullable reference types without initializer are required

        return propInfo;



        static (bool, string?) GetInitilializerInfo(IPropertySymbol p)
        {
            // Inspect syntax to find initializer and 'required' token usage for more precise info (nullable, default)
            foreach (var declRef in p.DeclaringSyntaxReferences)
            {
                var node = declRef.GetSyntax();

                if (node is PropertyDeclarationSyntax pds)
                {
                    if (pds.Initializer is { Value: var init })
                    {
                        // Record the initializer source text (useful for marking that a default exists)
                        return (true, init.ToString());
                    }

                    // If nullable annotation is present on syntax (e.g., string?) capture that too (fallback)
                    // (we already have semantic NullableAnnotation)
                }
            }
            return (false, null);
        }
    }

}
