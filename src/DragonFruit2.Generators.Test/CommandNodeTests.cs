using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DragonFruit2.Generators.Test;

// TODO: Update this class once CommandInfo tests pass
//public class CommandNodeTests
//{
//    [Theory]
//    [InlineData("[CommandClass] public class MyCommand { }", "MyCommand")]
//    [InlineData("[CommandClass] public abstract class BaseCommand { }", "BaseCommand")]
//    [InlineData("[CommandClass] internal class InternalCommand { }", "InternalCommand")]
//    [InlineData("[CommandClass] protected class ProtectedCommand { }", "ProtectedCommand")]
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
//            public class BaseCommand { }
//            public class DerivedCommand : BaseCommand { }
//            """;
//        var compilation = TestHelpers.GetCompilation(source, "");
//        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.DerivedCommand");
//        Assert.NotNull(typeSymbol);

//        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, "BaseCommand", "TestCli");

//        Assert.Equal("DerivedCommand", result.Name);
//        Assert.Equal("BaseCommand", result.BaseName);
//        Assert.Equal("BaseCommand", result.RootName);
//    }

//    [Fact]
//    // TODO: This test does not do what it says it does
//    public void CreateCommandInfo_WithNullCliNamespace_IsNull()
//    {
//        var source = "namespace TestNamespace; public class MyCommand { }";

//        var result = TestHelpers.CommandInfoFromSource(source, "");

//        Assert.Null(result.CliNamespaceName);
//    }

//    [Fact]
//    public void CreateCommandInfo_WithGenericBaseClass_ExtractsBaseName()
//    {
//        var source = """
//            namespace TestNamespace;
//            public abstract class BaseCommand<T> { }
//            public class DerivedCommand : BaseCommand<int> { }
//            """;
//        var commandSyntaxTree = CSharpSyntaxTree.ParseText(source);
//        var compilation = TestHelpers.GetCompilation(commandSyntaxTree);
//        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.DerivedCommand");
//        Assert.NotNull(typeSymbol);

//        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, "BaseCommand", "TestCli");

//        Assert.NotNull(result.BaseName);
//        Assert.Equal("BaseCommand", result.BaseName);
//    }

//[Fact]
//public void SubCommandCanBeCreated()
//{
//    var sourceText = """
//            public partial class MyCommand
//            {
//                public required string Name { get; set; }
//            }

//            public partial class MorningGreetingCommand : MyCommand
//            {
//            }

//            public partial class EveningGreetingCommand : MyCommand
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