using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DragonFruit2.Generators.Test;

public class PropInfoBuildingTests
{

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
    public async Task PropertiesMarkedWithAttributeAreArgumentss()
    {
        var sourceText = """
        public partial class MyArgs
        { 
            [DragonFruit2.Argument(1)]
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
        Assert.Equal("--given-name", commandInfo.Options.Single().CliName);

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
        Assert.True(commandInfo.Options.Single().HasInitializer);

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
        Assert.Equal("42", commandInfo.Options.Single().InitializerText);

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
        Assert.False(commandInfo.Options.Single().IsValueType);

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
    public void CreatePropInfo_MultipleProperties_CreatesAllPropInfos()
    {
        var source = """
            namespace TestNamespace;
            public class MyArgs
            {
                public string Name { get; set; }
                public int Age { get; set; }
                public string Email { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbols = typeSymbol.GetMembers().OfType<IPropertySymbol>().ToList();

        var results = propSymbols.Select(x => PropInfoHelpers.CreatePropInfo(x, compilation.GetSemanticModel(argsTree))).ToList();

        Assert.Equal(3, results.Count);
        Assert.Equal("Name", results[0].Name);
        Assert.Equal("Age", results[1].Name);
        Assert.Equal("Email", results[2].Name);
    }


}
