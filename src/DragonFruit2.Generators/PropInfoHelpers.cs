using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Reflection.Metadata;

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

        var ctorArgumentLookup = GetCtorArgumentLookup(validationAttribute);
        if (ctorArgumentLookup == null) return null;

        // TODO: Support multiple validator type constructors, as soon as we figure out how to select the right one
        // These are the parameters to the validator type's constructor
        var validatorCtorParameters = GetValidatorParameters(validationAttribute);
        if (validatorCtorParameters == null) return null;

        var argumentInfos = new ValidatorArgumentInfo[validatorCtorParameters.Count() - 1]; // The first parameters is the DataValue (also property) name

        var arguments = new ValidatorArgumentInfo[validatorCtorParameters.Length];

        for (int i = 1; i < validatorCtorParameters.Length; i++)  // Loop starts at zero, becasue first value is DataValue.Name
        {
            var validatorParameter = validatorCtorParameters[i + 1];  // Offset for first param being DataValue.Name
            var name = validatorParameter.Name;
            var validatorType = validatorParameter.Type;
            if (!ctorArgumentLookup.TryGetValue(name, out var argumentTypeAndValue)) return null;
                // TODO: Should we add a warning diagnostic or fail if the types do not match. We may not be able to handle implicit conversions here. Or should we rely on the analyzer and optize here. We are passing the semantic model a long way for this.
                if (!semanticModel.Compilation.ClassifyConversion(validatorType, argumentTypeAndValue.argumentType).IsImplicit) return null;
            var value = argumentTypeAndValue.value;
            arguments[i] = new ValidatorArgumentInfo
            {
                Name = name,
                ValidatorTypeName = validatorType.ToString(),
                ArgumentTypeName = argumentTypeAndValue.argumentType.ToString(),
                Value = value
            }; 
        }

        return new ValidatorInfo
        {
            AttributeName = AttributeClassName(attrClass),
            ValidatorName = validatorType.Name.ToString(),
            ValidatorValues = null,
        };

        static int? CtorPosition(ImmutableArray<IParameterSymbol> attributeCtorParameters, string name)
        {
            for (int i = 0; i < attributeCtorParameters.Count(); i++)
            {
                if (attributeCtorParameters[i].Name == name)
                { return i; }
            }
            return null;
        }
    }

    private static IParameterSymbol[]? GetValidatorParameters(AttributeData validationAttribute)
    {
        // ctor in this method refers to the validator type's constructor
        var attrClass = validationAttribute.AttributeClass;
        if (attrClass is null) return null;

        var validatorType = GetValidatorType(attrClass);
        if (validatorType is null) return null;

        var members = validatorType.GetMembers();
        members = members.Any()
            ? members
                    : validatorType.OriginalDefinition.GetMembers();

        // TODO: Determine mechansm to support multile validator ctors, as that is a likely use case
        var validatorCtor = members
                .OfType<IMethodSymbol>()
                .FirstOrDefault(m => m.MethodKind == MethodKind.Constructor);
        return validatorCtor.Parameters.ToArray();
        static INamedTypeSymbol? GetValidatorType(INamedTypeSymbol attrClass)
        {
            var infoAttribute = attrClass.GetAttributes().Where(x => x.AttributeClass?.Name == "ValidatorAttributeInfo").FirstOrDefault();
            var validatorTypeConstant = infoAttribute?.ConstructorArguments.First();
            var validatorTypeAsObject = validatorTypeConstant?.Value;
            if (validatorTypeAsObject is INamedTypeSymbol validatorType)
                return validatorType;
            return null;
        }
    }



    private static Dictionary<string, (ITypeSymbol argumentType, string value)>? GetCtorArgumentLookup(AttributeData validationAttribute)
    {
        // ctor in this class refers to the Attribute's contructor
        var ctor = validationAttribute.AttributeConstructor;
        if (ctor is null) return null;
        var parameters = ctor.Parameters;

        var arguments = validationAttribute.ConstructorArguments;

        Dictionary<string, (ITypeSymbol argumentType, string value)> ret = new();
        for (int i = 0; i < parameters.Length; i++)
        {
            ret.Add(parameters[i].Name, (parameters[i].Type, arguments[i].ToCSharpString()));
        }
        return ret;
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
