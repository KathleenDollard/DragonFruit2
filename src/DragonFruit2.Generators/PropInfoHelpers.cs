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

        propInfo.Validators.AddRange(GetValidators(propSymbol.GetAttributes(), semanticModel, propInfo));
        propInfo.Defaults.AddRange(GetDefaults(propSymbol.GetAttributes(), semanticModel, propInfo));

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


    private static IEnumerable<ValidatorInfo> GetValidators(IEnumerable<AttributeData> attributes, SemanticModel semanticModel, PropInfo propInfo)
    {
        var validators = new List<ValidatorInfo>();
        var validationAttributePairs = GetMatchingAttributes(attributes, "ValidatorAttributeAttribute");

        foreach (var (validationAttribute, validationAttrAttr) in validationAttributePairs)
        {
            var validatorInfo = GetValidatorInfo(validationAttribute, validationAttrAttr, semanticModel);
            if (validatorInfo != null)
            {
                validators.Add(validatorInfo);
            }
        }
        return validators;
    }


    /// <summary>
    /// Extract validator metadata from an AttributeData.
    /// This reads the attribute class, constructor arguments and named arguments.
    /// The returned ValidatorInfo is populated with the attribute name, full type name,
    /// a list of constructor-argument string representations, and a dictionary of named args.
    /// </summary>
    private static ValidatorInfo? GetValidatorInfo(AttributeData validationAttribute, AttributeData validationAttrAttr, SemanticModel semanticModel)
    {
        if (validationAttribute is null) return null;

        var ctorArgumentLookup = GetCtorArgumentLookup(validationAttribute);
        if (ctorArgumentLookup == null) return null;

        var attrClass = validationAttribute.AttributeClass;
        if (attrClass == null) return null;

        var validatorType = GetTypeForAttributeParameter(attrClass, validationAttrAttr);
        if (validatorType == null) return null;

        // TODO: Support multiple validator type constructors, as soon as we figure out how to select the right one
        // These are the parameters to the validator type's constructor
        // The first parameter to the validator type constructor is the property name
        var validatorCtorParameters = GetParameters(validationAttribute, validatorType).Skip(1).ToArray();
        if (validatorCtorParameters == null) return null;

        var arguments = new ArgumentInfo[validatorCtorParameters.Length];
        for (int i = 0; i < validatorCtorParameters.Length; i++)
        {
            var validatorParameter = validatorCtorParameters[i];
            var name = validatorParameter.Name;
            var validatorParameterType = validatorParameter.Type;
            if (!ctorArgumentLookup.TryGetValue(name, out var argumentTypeAndValue)) return null;
            // TODO: Should we add a warning diagnostic or fail if the types do not match. We may not be able to handle implicit conversions here. Or should we rely on the analyzer and optize here. We are passing the semantic model a long way for this.
            // It's a little more complicated because `semanticModel.Compilation.ClassifyConversion(argumentTypeAndValue.argumentType, validatorParameterType` will fail on common scenarios like `object` passed to `TValue`
            var value = argumentTypeAndValue.value;
            arguments[i] = new ArgumentInfo
            {
                Name = name,
                ParameterTypeName = validatorParameterType.ToString(),
                AttributeArgumentTypeName = argumentTypeAndValue.argumentType.ToString(),
                Value = value is TypedConstant tc ? TypedConstantToString(tc) : ConstantToString(value)
            };
        }

        return new ValidatorInfo
        {
            AttributeName = AttributeClassName(attrClass),
            ValidatorTypeName = validatorType.Name.ToString(),
            ValidatorArguments = arguments,
        };

    }


    private static IEnumerable<DefaultInfo> GetDefaults(ImmutableArray<AttributeData> attributes, SemanticModel semanticModel, PropInfo propInfo)
    {
        var defaults = new List<DefaultInfo>();
        var defaultAttributePairs = GetMatchingAttributes(attributes, "DefaultAttributeAttribute");

        foreach (var (defaultAttribute, defaultAttrAttr) in defaultAttributePairs)
        {
            var defaultInfo = GetDefaultInfo(defaultAttribute, defaultAttrAttr, semanticModel);
            if (defaultInfo != null)
            {
                defaults.Add(defaultInfo);
            }
        }
        return defaults;
    }

    private static IEnumerable<(AttributeData attribute, AttributeData attributeAttribute)>
        GetMatchingAttributes(IEnumerable<AttributeData> attributes, string attributeName)
    {
        var ret = new List<(AttributeData attribute, AttributeData attributeAttribute)>();
        foreach (var attribute in attributes)
        {
            var attrClass = attribute.AttributeClass;
            if (attrClass == null) continue;
            var matchingAttribute = attrClass
                .GetAttributes()
                .FirstOrDefault(x => x?.AttributeClass?.Name == attributeName);
            if (matchingAttribute is null) continue;

            ret.Add((attribute, matchingAttribute));
        }
        return ret;
    }

    private static DefaultInfo? GetDefaultInfo(AttributeData defaultAttribute, AttributeData defaultAttrAttr, SemanticModel semanticModel)
    {
        if (defaultAttribute is null) return null;

        var ctorArgumentLookup = GetCtorArgumentLookup(defaultAttribute);
        if (ctorArgumentLookup == null) return null;

        var attrClass = defaultAttribute.AttributeClass;
        if (attrClass == null) return null;

        var defaultType = GetTypeForAttributeParameter(attrClass, defaultAttrAttr);
        if (defaultType == null) return null;

        // TODO: Support multiple validator type constructors, as soon as we figure out how to select the right one
        // These are the parameters to the validator type's constructor
        // The first parameter to the validator type constructor is the property name
        var defaultCtorParameters = GetParameters(defaultAttribute, defaultType).Skip(1).ToArray();
        if (defaultCtorParameters == null) return null;

        var arguments = new ArgumentInfo[defaultCtorParameters.Length];
        for (int i = 0; i < defaultCtorParameters.Length; i++)
        {
            var validatorParameter = defaultCtorParameters[i];
            var name = validatorParameter.Name;
            var validatorParameterType = validatorParameter.Type;
            if (!ctorArgumentLookup.TryGetValue(name, out var argumentTypeAndValue)) return null;
            // TODO: Should we add a warning diagnostic or fail if the types do not match. We may not be able to handle implicit conversions here. Or should we rely on the analyzer and optize here. We are passing the semantic model a long way for this.
            // It's a little more complicated because `semanticModel.Compilation.ClassifyConversion(argumentTypeAndValue.argumentType, validatorParameterType` will fail on common scenarios like `object` passed to `TValue`
            var value = argumentTypeAndValue.value;
            arguments[i] = new ArgumentInfo
            {
                Name = name,
                ParameterTypeName = validatorParameterType.ToString(),
                AttributeArgumentTypeName = argumentTypeAndValue.argumentType.ToString(),
                Value = value is TypedConstant tc ? TypedConstantToString(tc) : ConstantToString(value)
            };
        }

        return new DefaultInfo
        {
            AttributeName = AttributeClassName(attrClass),
            DefaultTypeName = defaultType.Name.ToString(),
            DefaultArguments = arguments,
        };
    }

    private static INamedTypeSymbol? GetTypeForAttributeParameter(INamedTypeSymbol attrClass, AttributeData attributeAttribute)
    {
        var validatorTypeConstant = attributeAttribute?.ConstructorArguments.First();
        var validatorTypeAsObject = validatorTypeConstant?.Value;
        if (validatorTypeAsObject is INamedTypeSymbol validatorType)
            return validatorType;
        return null;
    }

    private static IEnumerable<IParameterSymbol>? GetParameters(AttributeData validationAttribute, ITypeSymbol validatorType)
    {
        // ctor in this method refers to the validator type's constructor
        var attrClass = validationAttribute.AttributeClass;
        if (attrClass is null) return null;

        var members = validatorType.GetMembers();
        members = members.Any()
            ? members
                    : validatorType.OriginalDefinition.GetMembers();

        // TODO: Determine mechansm to support multile validator ctors, as that is a likely use case
        var validatorCtor = members
                .OfType<IMethodSymbol>()
                .FirstOrDefault(m => m.MethodKind == MethodKind.Constructor);
        return [.. validatorCtor.Parameters];
    }

    private static Dictionary<string, (ITypeSymbol argumentType, object value)>? GetCtorArgumentLookup(AttributeData validationAttribute)
    {
        // ctor in this class refers to the Attribute's contructor
        var ctor = validationAttribute.AttributeConstructor;
        if (ctor is null) return null;
        var parameters = ctor.Parameters;

        var arguments = validationAttribute.ConstructorArguments;

        Dictionary<string, (ITypeSymbol argumentType, object value)> ret = [];
        for (int i = 0; i < parameters.Length; i++)
        {
            ret.Add(parameters[i].Name, (parameters[i].Type, arguments[i]));
        }
        return ret;
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

        return ConstantToString(tc.Value);
    }

    static string ConstantToString(object? value)
        => value switch
        {
            string s => $"\"{s}\"",
            char c => $"'{c}'",
            bool b => b ? "true" : "false",
            IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture) ?? value.ToString() ?? "null",
            _ => value?.ToString() ?? "null",

        };
}
