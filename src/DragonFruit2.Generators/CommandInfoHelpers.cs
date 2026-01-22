using DragonFruit2.GeneratorSupport;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            CliNamespaceName= cliNamespaceName,
            ArgsAccessibility = typeSymbol.DeclaredAccessibility.ToCSharpString(),
            BaseName = baseTypeName,
            RootName = rootName,
            IsStruct = typeSymbol.IsValueType,
        };
    }

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
            ContainingTypeName = propSymbol.ContainingType.Name,
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

        var validationAttributes = propSymbol.GetAttributes()
            .Where(x => x.AttributeClass?.BaseType?.Name == "ValidatorAttribute")
            .Select(x => x);
        foreach (var validationAttribute in validationAttributes)
        {
            var validatorInfo = GetValidatorInfo(validationAttribute);
            if (validatorInfo != null)
            {
                propInfo.Validators.Add(validatorInfo);
            }
        }


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

    /// <summary>
    /// Extract validator metadata from an AttributeData.
    /// This reads the attribute class, constructor arguments and named arguments.
    /// The returned ValidatorInfo is populated with the attribute name, full type name,
    /// a list of constructor-argument string representations, and a dictionary of named args.
    /// </summary>
    private static ValidatorInfo? GetValidatorInfo(AttributeData validationAttribute)
    {
        if (validationAttribute is null) return null;
        var attrClass = validationAttribute.AttributeClass;
        if (attrClass is null) return null;

        var info = new ValidatorInfo
        {

            Name = AttributeClassName(attrClass),
            FullTypeName = attrClass.ToDisplayString()
        };

        // Constructor arguments
        var ctorArgs = validationAttribute.ConstructorArguments
            .Select(TypedConstantToString)
            .ToList();

        // Named arguments (e.g., named properties set on the attribute)
        var namedArgs = validationAttribute.NamedArguments
            .ToDictionary(kv => kv.Key, kv => TypedConstantToString(kv.Value));

        // Attempt to populate expected ValidatorInfo fields if they exist.
        // Common ValidatorInfo shape: Name, FullTypeName, ConstructorArguments, NamedArguments.
        // Populate them defensively via available properties.
        try
        {
            // These properties are commonly present; adjust names if your ValidatorInfo differs.
            info.ConstructorArguments = ctorArgs.Select(x=>x.Trim()).ToList();
            info.NamedArguments = namedArgs;
        }
        catch
        {
            // If ValidatorInfo doesn't expose those members, silently keep minimal info (name/full type).
            // Consumer can call a helper to read constructor args directly from AttributeData if needed.
        }

        static string AttributeClassName(INamedTypeSymbol attrClass)
        {
            var attributeName = attrClass.Name;
            var attribute = "Attribute";
            if (attributeName.EndsWith(attribute))
            {
                attributeName = attributeName.Substring(0, attributeName.Length - attribute.Length);
            }
            return attributeName;
        }

        // Convert TypedConstant to readable string representation
        static string TypedConstantToString(TypedConstant tc)
        {
            if (tc.IsNull) return "null";

            if (tc.Kind == TypedConstantKind.Array)
            {
                var elems = tc.Values.Select(TypedConstantToString);
                return "[" + string.Join(", ", elems) + "]";
            }

            if (tc.Value is string s) return $"\"{s}\"";
            if (tc.Value is char c) return $"'{c}'";
            if (tc.Value is bool b) return b ? "true" : "false";
            if (tc.Value is IFormattable f) return f.ToString(null, System.Globalization.CultureInfo.InvariantCulture) ?? tc.Value.ToString() ?? "null";
            return tc.Value?.ToString() ?? "null";
        }


        return info;
    }
}
