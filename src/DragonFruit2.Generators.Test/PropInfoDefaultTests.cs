using DragonFruit2.Defaults;
using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DragonFruit2.Generators.Test;

public class PropInfoDefaultTests
{
    [Fact]
    public void CreatePropInfo_PropertyWithDefaultAttribute_AddsDefault()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Defaults;
            
            public class MyArgs
            {
                [DefaultConstant(13)]
                public int Age { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        // TODO: This test is failing because GetDefaultType is failing 
        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.NotEmpty(propInfo.Defaults);
        Assert.Single(propInfo.Defaults);
    }

    [Fact]
    public void DefaultExtraction_PopulatesDefaultName()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Defaults;
            
            public class MyArgs
            {
                [DefaultConstant(13)]
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

        Assert.Single(propInfo.Defaults);
        var defaultDefinition = propInfo.Defaults.First();
        Assert.Equal("DefaultConstant", defaultDefinition.DefaultTypeName);
    }

    [Fact]
    public void CreatePropInfo_DefaultExtraction_PopulatesConstructorArguments()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Defaults;
            
            public class MyArgs
            {
                [DefaultConstant(13)]
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

        var defaultDefinition = propInfo.Defaults.First();
        Assert.Equal("defaultValue", defaultDefinition.DefaultArguments.First().Name);
        Assert.Equal("13", defaultDefinition.DefaultArguments.First().Value);
    }

    [Fact]
    public void CreatePropInfo_MultipleDefaults_AddsAllDefaults()
    {
        // This test simmulates two non-ordered validations such as one depending on one other DataValue
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Defaults;

            public class MyArgs
            {
                [DefaultConstant("Other")]
                [DefaultConstant(15)]
                public int Age { get; set; }

                [DefaultConstant(30)]
                public int Other { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var propInfo = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.Equal(2, propInfo.Defaults.Count);
        var defaultValues = propInfo.Defaults.Select(v => v.DefaultArguments.First().Value).ToList();
        Assert.Equal("\"Other\"", defaultValues.First());
        Assert.Equal("15", defaultValues.Last());
    }

    [Fact]
    public void CreatePropInfo_DefaultWithStringArgument_QuotesStringValue()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Defaults;

            public class MyArgs
            {
                [DefaultConstant("Hello")]
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

        var defaultDefinition = propInfo.Defaults.First();
        Assert.Equal("\"Hello\"", defaultDefinition.DefaultArguments.First().Value);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithoutDefaults_DefaultsEmpty()
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

        Assert.Empty(propInfo.Defaults);
    }


    [Fact]
    public void CreatePropInfo_DefaultWithBooleanArgument_ConvertsTrueCorrectly()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Defaults;

            public class MyArgs
            {
                [DefaultConstant(true)]
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

        var defaultDefinition = propInfo.Defaults.First();
        Assert.Equal("true", defaultDefinition.DefaultArguments.First().Value);
    }

    [Fact]
    public void CreatePropInfo_DefaultWithFalseArgument_ConvertsFalseCorrectly()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Defaults;

            public class MyArgs
            {
                [DefaultConstant(false)]
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

        var defaultDefinition = propInfo.Defaults.First();
        Assert.Equal("false", defaultDefinition.DefaultArguments.First().Value);
    }

    [Fact]
    public void CreatePropInfo_DefaultWithNullArgument_ConvertsNullCorrectly()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2.Defaults;

            public class MyArgs
            {
                [DefaultConstant(null)]
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

        var defaultDefinition = propInfo.Defaults.First();
        Assert.Equal("null", defaultDefinition.DefaultArguments.First().Value);
    }
}
