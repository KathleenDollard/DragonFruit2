using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators.Test;

public class CommandInfoTests
{

    [Fact]
    public async Task CommandInfoCreatedFromClass()
    {
        var sourceText = """
            using DragonFruit2;

            [CommandClass] 
            public partial class MyCommand
            { }
            """;

        var commandInfo = TestHelpers.GetCommandInfo(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.Equal("MyCommand", commandInfo?.Name);
    }

    [Fact]
    public async Task CommandInfoIncludesCurlyNamespace()
    {
        var sourceText = """
            using DragonFruit2;

            namespace MyNamespace
            {
                [CommandClass] 
                public partial class MyCommand
                { }
            }
            """;

        var commandInfo = TestHelpers.GetCommandInfo(sourceText, TestHelpers.EmptyConsoleAppCodeWithArgsMyNamespace);

        Assert.NotNull(commandInfo);
        Assert.Equal("MyNamespace", commandInfo?.NamespaceName);
    }

    [Fact]
    public async Task CommandInfoIncludesSemicolonNamespace()
    {
        var sourceText = """
            using DragonFruit2;
            
            namespace MyNamespace;
            [CommandClass] 
            public partial class MyCommand
            { }
            """;

        var commandInfo = TestHelpers.GetCommandInfo(sourceText, TestHelpers.EmptyConsoleAppCodeWithArgsMyNamespace);

        Assert.NotNull(commandInfo);
        Assert.Equal("MyNamespace", commandInfo?.NamespaceName);
    }

    [Fact]
    public async Task CommandInfoIncludesArgsClassAccessibility()
    {
        var sourceText = """
            using DragonFruit2;
            
            [CommandClass] 
            public partial class MyCommand
            { }
            """;

        var commandInfo = TestHelpers.GetCommandInfo(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Equal("public", commandInfo?.Accessibility);
    }


    [Fact]
    public async Task CommandInfoIncludesArgsClassTwoWordAccessibility()
    {
        var sourceText = """
            using DragonFruit2;
            
            [CommandClass] 
            protected internal partial class MyCommand
            { }
            """;

        var commandInfo = TestHelpers.GetCommandInfo(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Equal("protected internal", commandInfo?.Accessibility);
    }


    [Theory]
    [InlineData("public", "public")]
    [InlineData("internal", "internal")]
    [InlineData("protected", "protected")]
    [InlineData("private", "private")]
    public void CreateCommandInfo_PreservesAccessibility(string accessibility, string expectedAccessibility)
    {
        var source = $$"""
            using DragonFruit2; 
            
            namespace TestNamespace; 
            [CommandClass] 
            {{accessibility}} class MyCommand  
            { }
            """;

        var result = TestHelpers.GetCommandInfo(source, "");

        Assert.Equal(expectedAccessibility, result?.Accessibility);
    }



    [Fact]
    public void CreateCommandInfo_WithoutNamespace_HasNullNamespaceName()
    {
        var source = $$"""
            using DragonFruit2; 

            [CommandClass] 
            public class MyCommand  
            { }
            """;

        var result = TestHelpers.GetCommandInfo(source, "");

        Assert.Null(result?.NamespaceName);
    }
}
