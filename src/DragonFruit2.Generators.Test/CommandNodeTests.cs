using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DragonFruit2.Generators.Test;

// TODO: Update this class once CommandInfo tests pass
//public class CommandNodeTests
//{
//    [Theory]
//    [InlineData("[CommandClass] public class MyArgs { }", "MyArgs")]
//    [InlineData("[CommandClass] public abstract class BaseArgs { }", "BaseArgs")]
//    [InlineData("[CommandClass] internal class InternalArgs { }", "InternalArgs")]
//    [InlineData("[CommandClass] protected class ProtectedArgs { }", "ProtectedArgs")]
//    public void CreateCommandInfo_RootType_HasNullBaseName(string typeDeclaration, string typeName)
//    {
//        var source = $"namespace TestNamespace; {typeDeclaration}";

//        var result = TestHelpers.CommandInfoFromSource(source, "");

//        Assert.Equal(typeName, result.Name);
//        Assert.Equal("TestNamespace", result.NamespaceName);
//        Assert.Equal("TestCli", result.CliNamespaceName);
//        Assert.Null(result.BaseName);
//        Assert.Equal(typeName, result.RootName);
//    }

//    [Fact]
//    public void CreateCommandInfo_DerivedType_HasBaseName()
//    {
//        var source = """
//            namespace TestNamespace;
//            public class BaseArgs { }
//            public class DerivedArgs : BaseArgs { }
//            """;
//        var compilation = TestHelpers.GetCompilation(source, "");
//        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.DerivedArgs");
//        Assert.NotNull(typeSymbol);

//        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, "BaseArgs", "TestCli");

//        Assert.Equal("DerivedArgs", result.Name);
//        Assert.Equal("BaseArgs", result.BaseName);
//        Assert.Equal("BaseArgs", result.RootName);
//    }

//    [Fact]
//    // TODO: This test does not do what it says it does
//    public void CreateCommandInfo_WithNullCliNamespace_IsNull()
//    {
//        var source = "namespace TestNamespace; public class MyArgs { }";

//        var result = TestHelpers.CommandInfoFromSource(source, "");

//        Assert.Null(result.CliNamespaceName);
//    }

//    [Fact]
//    public void CreateCommandInfo_WithGenericBaseClass_ExtractsBaseName()
//    {
//        var source = """
//            namespace TestNamespace;
//            public abstract class BaseArgs<T> { }
//            public class DerivedArgs : BaseArgs<int> { }
//            """;
//        var argsTree = CSharpSyntaxTree.ParseText(source);
//        var compilation = TestHelpers.GetCompilation(argsTree);
//        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.DerivedArgs");
//        Assert.NotNull(typeSymbol);

//        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, "BaseArgs", "TestCli");

//        Assert.NotNull(result.BaseName);
//        Assert.Equal("BaseArgs", result.BaseName);
//    }

//[Fact]
//public void SubCommandCanBeCreated()
//{
//    var sourceText = """
//            public partial class MyArgs
//            {
//                public required string Name { get; set; }
//            }

//            public partial class MorningGreetingArgs : MyArgs
//            {
//            }

//            public partial class EveningGreetingArgs : MyArgs
//            {
//                public int Age { get; init; } = 1;
//            }
//            """;
//    var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

//    Assert.NotNull(commandInfo);
//    Assert.Single(commandInfo.Options);
//    Assert.Equal(2, commandInfo.SubCommands.Count);
//    var subMorning = commandInfo.SubCommands[0];
//    Assert.NotNull(subMorning);
//    Assert.Empty(subMorning.Options);
//    var subEvening = commandInfo.SubCommands[1];
//    Assert.NotNull(subEvening);
//    Assert.Single(subEvening.Options);


//}

//}