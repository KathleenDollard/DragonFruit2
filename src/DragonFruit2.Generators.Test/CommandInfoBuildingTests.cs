using Microsoft.CodeAnalysis;
using Xunit;

namespace DragonFruit2.Generators.Test;

public class CommandInfoBuildingTests
{
    #region CommandInfo Creation Tests
    [Fact]
    public async Task CommandInfoCreatedFromClass()
    {
        var sourceText = """
            public partial class MyArgs
            { }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation( invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.Equal("MyArgs", commandInfo?.Name);
        Assert.False(commandInfo?.IsStruct);
    }

    [Fact]
    public async Task CommandInfoCreatedFromStruct()
    {
        var sourceText = """
            public partial struct MyArgs
            { }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.Equal("MyArgs", commandInfo?.Name);
        Assert.True(commandInfo?.IsStruct);
    }

    [Fact]
    public async Task CommandInfoIncludesCurlyNamespace()
    {
        var sourceText = """
            namespace MyNamespace
            {
                public partial class MyArgs
                { }
            }
            """;
      
        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCodeWithArgsMyNamespace);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Equal("MyNamespace", commandInfo?.NamespaceName);
    }

    [Fact]
    public async Task CommandInfoIncludesSemicolonNamespace()
    {
        var sourceText = """
            namespace MyNamespace;
            public partial class MyArgs
            { }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCodeWithArgsMyNamespace);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Equal("MyNamespace", commandInfo?.NamespaceName);
    }

    [Fact]
    public async Task CommandInfoIncludesArgsClassAccessibility()
    {
        var sourceText = """
            public partial class MyArgs
            { }
            """;
        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));
        
        Assert.NotNull(commandInfo);
        Assert.Equal("public", commandInfo?.ArgsAccessibility);
    }


    [Fact]
    public async Task CommandInfoIncludesArgsClassTwoWordAccessibility()
    {
        var sourceText = """
            protected internal partial class MyArgs
            { }
            """;
        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Equal("public", commandInfo?.ArgsAccessibility);
    }
    #endregion

    #region PropInfo Creation Tests
    [Fact]
    public async Task PropertiesDefaultToOptions()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public required string Name { get; set; }
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Empty(commandInfo.Arguments);

    }

    [Fact]
    public async Task PropertiesMarkedWithAttributeAreArgs()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                [DragonFruit2.Argument(Position = 1)]
                public required string Name { get; set; }
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Arguments);
        Assert.Empty(commandInfo.Options);
        Assert.Equal(1, commandInfo.Arguments.Single().Position);

    }

    [Fact]
    public async Task PropInfoHasName()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public int Name { get; set; }
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("Name", commandInfo.Options.Single().Name);

    }

    [Fact]
    public async Task PropInfoHasCliNameWithKebabCase()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public int GivenName { get; set; }
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("given-name", commandInfo.Options.Single().CliName);

    }

    [Fact]
    public async Task PropInfoHasTypeName()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public int Name { get; set; }
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("int", commandInfo.Options.Single().TypeName);

    }


    [Fact]
    public async Task PropInfoRecognizesRequiredModifier()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public required string Name { get; set; }
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("Name", commandInfo.Options.Single().Name);
        Assert.True(commandInfo.Options.Single().HasRequiredModifier);

    }

    [Fact]
    public async Task PropInfoRecognizesLackOfRequiredModifier()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public string Name { get; set; }
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("Name", commandInfo.Options.Single().Name);
        Assert.False(commandInfo.Options.Single().HasRequiredModifier);

    }


    [Fact]
    public async Task PropInfoRecognizesInitializer()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public int Name { get; set; } = 42
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.True (commandInfo.Options.Single().HasInitializer);

    }

    [Fact]
    public async Task PropInfoHasInitializationText()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public int Name { get; set; } = 42
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("42",commandInfo.Options.Single().InitializerText);

    }

    [Fact]
    public async Task PropInfoHasValueTypeTrueForValueType()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public int Name { get; set; } = 42
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.True(commandInfo.Options.Single().IsValueType);

    }

    [Fact]
    public async Task PropInfoHasValueTypeFalseForReferenceType()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public string Name { get; set; } = 42
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.False( commandInfo.Options.Single().IsValueType);

    }

    [Fact]
    public async Task PropInfoHasNullableAnnotatedWhenPresent()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public string? Name { get; set; } = 42
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal(NullableAnnotation.Annotated, commandInfo.Options.Single().NullableAnnotation);

    }

    [Fact]
    public async Task PropInfoHasFalseNullableNoneWhenAbsent()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                public string Name { get; set; } = 42
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal(NullableAnnotation.None, commandInfo.Options.Single().NullableAnnotation);

    }

    [Fact]
    public async Task PropInfoHasDescriptionFromAttribute()
    {
        var sourceText = """
            public partial class MyArgs
            { 
                [DragonFruit2.Description("R2D2")]
                public string Name { get; set; } = 42
            }
            """;

        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("R2D2", commandInfo.Options.Single().Description);

    }

    [Fact]
    public async Task PropInfoSingleValidatorWithCtorParameters()
    {
        var sourceText = """
            using DragonFruit2.Validators;
            using DragonFruit2.Generators.Test;;

            public partial class MyArgs
            { 
                [ValidatorTestAttributeOneCtorParam(0)]
                public int Age { get; set; } = 42;
            }
            """;
        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        var propInfo = commandInfo.Options.Single();
        Assert.Single(propInfo.Validators);
        var validator = propInfo.Validators.Single();
        Assert.Equal("ValidatorTestAttributeOneCtorParam", validator.Name);
        Assert.Single(validator.ConstructorArguments);
        Assert.Equal("0", validator.ConstructorArguments[0]);
    }


    [Fact]
    public async Task PropInfoSingleValidatorWithNamedArguments()
    {
        var sourceText = """
            using DragonFruit2.Validators;
            using DragonFruit2.Generators.Test;;
            
            public partial class MyArgs
            { 
                [ValidatorTestAttributeOneNamedParam(AnotherValue = 2)]
                public int Age { get; set; } = 42;
            }
            """;
        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        var propInfo = commandInfo.Options.Single();
        Assert.Single(propInfo.Validators);
        var validator = propInfo.Validators.Single();
        Assert.Single(validator.NamedArguments);
        Assert.Equal("AnotherValue", validator.NamedArguments.Keys.Single());
        Assert.Equal("2", validator.NamedArguments.Values.Single());
    }


    [Fact]
    public async Task PropInfoMultipleValidators()
    {
        var sourceText = """
            using DragonFruit2.Validators;
            using DragonFruit2.Generators.Test;;
            
            public partial class MyArgs
            { 
                [ValidatorTestAttributeOneCtorParam(0)]
                [ValidatorTestAttributeOneNamedParam(0, AnotherValue = 2)]
                [ValidatorTestAttributeMulitpleMixedParams(0, "42", AnotherValue = 2, StillAnother = 43)]
                public int Age { get; set; } = 42;
            }
            """;
        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        var propInfo = commandInfo.Options.Single();
        Assert.Equal(3, propInfo.Validators.Count);
        var validator = propInfo.Validators[0];
    }
    #endregion
}
