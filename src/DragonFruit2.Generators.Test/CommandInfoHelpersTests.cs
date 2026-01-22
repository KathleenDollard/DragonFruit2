using DragonFruit2.Generators;
using DragonFruit2.GeneratorSupport;
using Microsoft.CodeAnalysis;
using Xunit;

namespace DragonFruit2.Generators.Test;

public class CommandInfoHelpersTests
{
    [Theory]
    [InlineData("public class MyArgs { }", "MyArgs")]
    [InlineData("public abstract class BaseArgs { }", "BaseArgs")]
    [InlineData("internal class InternalArgs { }", "InternalArgs")]
    [InlineData("protected class ProtectedArgs { }", "ProtectedArgs")]
    public void CreateCommandInfo_RootType_HasNullBaseName(string typeDeclaration, string typeName)
    {
        var source = $"namespace TestNamespace; {typeDeclaration}";
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace." + typeName);
        Assert.NotNull(typeSymbol);

        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, typeName, "TestCli");

        Assert.Equal(typeName, result.Name);
        Assert.Equal("TestNamespace", result.NamespaceName);
        Assert.Equal("TestCli", result.CliNamespaceName);
        Assert.Null(result.BaseName);
        Assert.Equal(typeName, result.RootName);
        Assert.False(result.IsStruct);
    }

    [Fact]
    public void CreateCommandInfo_DerivedType_HasBaseName()
    {
        var source = """
            namespace TestNamespace;
            public class BaseArgs { }
            public class DerivedArgs : BaseArgs { }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.DerivedArgs");
        Assert.NotNull(typeSymbol);

        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, "BaseArgs", "TestCli");

        Assert.Equal("DerivedArgs", result.Name);
        Assert.Equal("BaseArgs", result.BaseName);
        Assert.Equal("BaseArgs", result.RootName);
    }

    [Fact]
    public void CreateCommandInfo_StructType_SetsIsStruct()
    {
        var source = "namespace TestNamespace; public struct MyArgs { }";
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);

        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, "MyArgs", "TestCli");

        Assert.True(result.IsStruct);
    }

    [Theory]
    [InlineData("public", "public")]
    [InlineData("internal", "internal")]
    [InlineData("protected", "protected")]
    [InlineData("private", "private")]
    public void CreateCommandInfo_PreservesAccessibility(string accessibility, string expectedAccessibility)
    {
        var source = $"namespace TestNamespace; {accessibility} class MyArgs {{ }}";
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);

        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, "MyArgs", "TestCli");

        Assert.Equal(expectedAccessibility, result.ArgsAccessibility);
    }

    [Fact]
    public void CreateCommandInfo_WithoutNamespace_HasNullNamespaceName()
    {
        var source = "public class MyArgs { }";
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("MyArgs");
        Assert.NotNull(typeSymbol);

        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, "MyArgs", "TestCli");

        Assert.Null(result.NamespaceName);
    }

    [Fact]
    public void CreateCommandInfo_WithNullCliNamespace_IsNull()
    {
        var source = "namespace TestNamespace; public class MyArgs { }";
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);

        var result = CommandInfoHelpers.CreateCommandInfo(typeSymbol, "MyArgs", null);

        Assert.Null(result.CliNamespaceName);
    }

    [Fact]
    public void CreatePropInfo_SimpleStringProperty_CreatesValidPropInfo()
    {
        var source = """
            namespace TestNamespace;
            public class MyArgs
            {
                public string Name { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.Equal("Name", result.Name);
        Assert.Equal("string", result.TypeName);
        Assert.Equal("MyArgs", result.ContainingTypeName);
        Assert.False(result.IsValueType);
        Assert.False(result.HasInitializer);
    }

    [Fact]
    public void CreatePropInfo_ValueTypeProperty_IsValueTypeTrue()
    {
        var source = """
            namespace TestNamespace;
            public class MyArgs
            {
                public int Age { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.True(result.IsValueType);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithInitializer_CapturesInitializer()
    {
        var source = """
            namespace TestNamespace;
            public class MyArgs
            {
                public string Greeting { get; set; } = "Hello";
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Greeting");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.True(result.HasInitializer);
        Assert.NotNull(result.InitializerText);
        Assert.Contains("Hello", result.InitializerText);
    }

    [Fact]
    public void CreatePropInfo_RequiredProperty_SetsHasRequiredModifier()
    {
        var source = """
            namespace TestNamespace;
            public class MyArgs
            {
                public required string Name { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.True(result.HasRequiredModifier);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithArgumentAttribute_SetsIsArgument()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;
            
            public class MyArgs
            {
                [Argument(Position = 0)]
                public string Name { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.True(result.HasArgumentAttribute);
        Assert.True(result.IsArgument);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithArgumentAttribute_CapturesPosition()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;
            
            public class MyArgs
            {
                [Argument(Position = 5)]
                public string Name { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.Equal(5, result.Position);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithDescriptionAttribute_CapturesDescription()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;
            
            public class MyArgs
            {
                [Description("User's full name")]
                public string Name { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.Equal("User's full name", result.Description);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithValidatorAttribute_AddsValidator()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Validators;
            
            public class MyArgs
            {
                [GreaterThan(0)]
                public int Age { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.NotEmpty(result.Validators);
        Assert.Single(result.Validators);
    }

    [Fact]
    public void CreatePropInfo_ValidatorExtraction_PopulatesValidatorName()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Validators;
            
            public class MyArgs
            {
                [GreaterThan(0)]
                public int Age { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.Single(result.Validators);
        var validator = result.Validators.First();
        Assert.Equal("GreaterThan", validator.Name);
    }

    [Fact]
    public void CreatePropInfo_ValidatorExtraction_PopulatesConstructorArguments()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Validators;
            
            public class MyArgs
            {
                [GreaterThan(0)]
                public int Age { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        var validator = result.Validators.First();
        Assert.NotEmpty(validator.ConstructorArguments);
        Assert.Contains("0", validator.ConstructorArguments);
    }

    [Fact]
    public void CreatePropInfo_MultipleValidators_AddsAllValidators()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Validators;
            
            public class MyArgs
            {
                [GreaterThan(0)]
                [LessThan(150)]
                public int Age { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.Equal(2, result.Validators.Count);
        var validatorNames = result.Validators.Select(v => v.Name).ToList();
        Assert.Contains("GreaterThan", validatorNames);
        Assert.Contains("LessThan", validatorNames);
    }

    [Fact]
    public void CreatePropInfo_NullableAnnotation_CapturesNullability()
    {
        var source = """
            namespace TestNamespace;
            #nullable enable
            
            public class MyArgs
            {
                public string? Name { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.Equal(NullableAnnotation.Annotated, result.NullableAnnotation);
    }

    [Fact]
    public void CreatePropInfo_ValidatorWithStringArgument_QuotesStringValue()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Validators;
            
            public class MyArgs
            {
                [MinLength("5")]
                public string Name { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        var validator = result.Validators.First();
        Assert.Contains("\"5\"", validator.ConstructorArguments);
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
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbols = typeSymbol.GetMembers().OfType<IPropertySymbol>().ToList();

        var results = propSymbols.Select(CommandInfoHelpers.CreatePropInfo).ToList();

        Assert.Equal(3, results.Count);
        Assert.Equal("Name", results[0].Name);
        Assert.Equal("Age", results[1].Name);
        Assert.Equal("Email", results[2].Name);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithoutInitializer_InitializerTextNull()
    {
        var source = """
            namespace TestNamespace;
            public class MyArgs
            {
                public string Name { get; set; }
            }
            """;
        var compilation = TestHelpers.GetCompilation(source, "");
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = CommandInfoHelpers.CreatePropInfo(propSymbol);

        Assert.False(result.HasInitializer);
        Assert.Null(result.InitializerText);
    }
}