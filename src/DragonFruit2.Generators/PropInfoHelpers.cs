using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace DragonFruit2.Generators;

public static class PropInfoHelpers
{
    /// <summary>
    /// Build model of properties using semantic and syntactic information available at compile time
    /// </summary>
    /// <param name="propSymbol"></param>
    /// <returns></returns>
    public static PropInfo CreatePropInfo(IPropertySymbol propSymbol, SemanticModel semanticModel)
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
            .Where(x => x.AttributeClass?.BaseType?.Name == "MemberValidatorAttribute")
            .Select(x => x);
        foreach (var validationAttribute in validationAttributes)
        {
            var validatorInfo = GetValidatorInfo(validationAttribute, semanticModel);
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
    private static ValidatorInfo? GetValidatorInfo(AttributeData validationAttribute, SemanticModel semanticModel)
    {
        if (validationAttribute is null) return null;
        var attrClass = validationAttribute.AttributeClass;
        if (attrClass is null) return null;

        #region Different approach
        var ctors = attrClass.GetMembers().OfType<IMethodSymbol>().Where(x=>x.MethodKind == MethodKind.Constructor);
        var infoAttribute = attrClass.GetAttributes().Where(x=>x.AttributeClass?.Name == "ValidatorAttributeInfo").FirstOrDefault();
        var validatorType = infoAttribute?.ConstructorArguments.First();
        #endregion

        var ctorFromAttribute = validationAttribute.AttributeConstructor;
        if (ctorFromAttribute is null)
        {
            // TODO: Add diagnostic, apparently the attribute does not have a constructor
            return null;
        }
        // TODO: Start here. The problem is that the DeclaringSyntaxReferences returns an empty list
        //       I wonder if this is because it is not in this syntax tree.
        var ctorReference = ctorFromAttribute.DeclaringSyntaxReferences.First(); // do not antiticipate partial constructors
        if (ctorReference.GetSyntax() is not ConstructorDeclarationSyntax ctorSyntax)
        {
            // TODO: Apparently an internal error
            return null;
        }
        var ctorInitializer = ctorSyntax.Initializer;
        if (ctorInitializer is null || ctorInitializer.Kind() != SyntaxKind.BaseConstructorInitializer)
        {
            // TODO: Register diagnostic, apparently there is not a base initializer on the attribute.
            return null;
        }
        var baseCtorArguments = ctorInitializer.ArgumentList.Arguments;
        var validatorTypeName = baseCtorArguments.FirstOrDefault()?.Expression.GetText(); // or possibly ToString()
        if (validatorType is null)
        {
            // TODO: Apparently the base ctor call does not pass an argument
            return null;
        }
        var availableValues = GetAvailableValues(validationAttribute);
        if (availableValues is null) return null; // if TODO:s are done, the diagnostic is already reported
        KeyValuePair<string, string>[]? parameters = GetParameters(ctorInitializer, availableValues, semanticModel);
        if (parameters is null) return null; // if TODO:s are done, the diagnostic is already reported

        return new ValidatorInfo
        {
            AttributeName = AttributeClassName(attrClass),
            ValidatorName = validatorType.ToString(),
            ValidatorValues = parameters
        };
    }

    private static Dictionary<string, string>? GetAvailableValues(AttributeData validationAttribute)
    {
        // Constructor arguments
        var ctorArgs = validationAttribute.ConstructorArguments
            .Select(TypedConstantToString)
            .ToList();
        var attrClass = validationAttribute.AttributeClass;
        var ctorParams = validationAttribute.AttributeConstructor?.Parameters.ToList();
        if (ctorParams is null || ctorArgs is null) return null;

        return ctorParams
                .Zip(ctorArgs, (p, a) => new KeyValuePair<string, string>(p.Name, a))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private static KeyValuePair<string, string>[]?
                GetParameters(ConstructorInitializerSyntax ctorInitializer,
                              Dictionary<string, string> availableValues,
                              SemanticModel semanticModel)
    {
        var baseCtorSymbol = GetBaseCtorSymbol(ctorInitializer, semanticModel);
        if (baseCtorSymbol is null)
        {
            // TODO: Register diagnostic, apparently no base class was found
            return null;
        }
        var parameters = baseCtorSymbol.Parameters.Select(p => new KeyValuePair<string, string>(p.Name, p.Type.Name)).ToArray();

        var retParameters = new KeyValuePair<string, string>[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var paramName = parameters[i].Key;
            if (availableValues.TryGetValue(paramName, out var value))
            {
                retParameters[i] = new KeyValuePair<string, string>(paramName, value);
                continue;
            }
            var pascalName = $"{char.ToUpperInvariant(paramName[0])}{paramName.Substring(1)}"; // Not using ToPascal from string extensions because we know we assume we have camel case to start, so this is easier
            if (availableValues.TryGetValue(pascalName, out value))
            {
                retParameters[i] = new KeyValuePair<string, string>(paramName, value);
                continue;
            }
            // TODO: Register diagnostic, apparently a parameter was not found by name
            return null;
        }
        return retParameters;

    }

    private static IMethodSymbol? GetBaseCtorSymbol(ConstructorInitializerSyntax ctorInitializer, SemanticModel semanticModel)
    {

        // Get the symbol information for the initializer
        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(ctorInitializer);

        // The symbol should be an IMethodSymbol representing the called constructor
        if (symbolInfo.Symbol is IMethodSymbol baseConstructorSymbol)
        {
            return baseConstructorSymbol;
        }
        return null;
    }

    private static string AttributeClassName(INamedTypeSymbol attrClass)
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
}
