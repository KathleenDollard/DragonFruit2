using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DragonFruit2.Generators.Test;

public class PropInfoValidatorTests
{
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
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.NotEmpty(propInfo.Validators);
        Assert.Single(propInfo.Validators);
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
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.Single(propInfo.Validators);
        var validator = propInfo.Validators.First();
        Assert.Equal("GreaterThanValidator", validator.ValidatorTypeName);
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
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        var validator = propInfo.Validators.First();
        Assert.NotEmpty(validator.ValidatorArguments);
        Assert.Equal("compareWithValue", validator.ValidatorArguments.First().Name);
        Assert.Equal("0", validator.ValidatorArguments.First().Value);
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
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.Equal(2, propInfo.Validators.Count);
        var validatorNames = propInfo.Validators.Select(v => v.ValidatorTypeName).ToList();
        Assert.Contains("GreaterThanValidator", validatorNames);
        Assert.Contains("LessThanValidator", validatorNames);
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
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        var validator = propInfo.Validators.First();
        Assert.Equal("\"5\"", validator.ValidatorArguments.First().Value);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithoutValidators_ValidatorsEmpty()
    {
        var source = """
            namespace TestNamespace;
            public class MyArgs
            {
                public string Name { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.Empty(propInfo.Validators);
    }

    [Fact()]
    public void CreatePropInfo_ValidatorWithBooleanArgument_ConvertsTrueCorrectly()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Validators;
            
            public class MyArgs
            {
                [ConstantValidator(true)]
                public string Name { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        var validator = propInfo.Validators.First();
        Assert.Equal("true", validator.ValidatorArguments.First().Value);
    }

    [Fact()]
    public void CreatePropInfo_ValidatorWithBooleanArgument_ConvertsFalseCorrectly()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Validators;
            
            public class MyArgs
            {
                [ConstantValidator(false)]
                public string Name { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        var validator = propInfo.Validators.First();
        Assert.Equal("false", validator.ValidatorArguments.First().Value);
    }

    [Fact]
    public void CreatePropInfo_ValidatorWithNullArgument_ConvertsNullCorrectly()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Validators;
            
            public class MyArgs
            {
                [ConstantValidator(null)]
                public int Age { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        var validator = propInfo.Validators.First();
        Assert.Equal("null", validator.ValidatorArguments.First().Value);
    }
}
