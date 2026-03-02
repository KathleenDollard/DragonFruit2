using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators.Test;

public class CommandInfoBuildingTests
{
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
        Assert.Equal("protected internal", commandInfo?.ArgsAccessibility);
    }
}
