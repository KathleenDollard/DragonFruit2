using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DragonFruit2.Generators.Test;

public class PropInfoBuildingTests
{

    [Fact]
    public async Task PropertiesDefaultToOptions()
    {
        var sourceText = """
        using DragonFruit2;
        
        [CommandClass] 
        public partial class MyArgs : CommandRootBase<MyArgs>
        { 
            public required string Name { get; set; }
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Empty(commandInfo.Arguments);

    }

    [Fact]
    public async Task PropertiesMarkedWithAttributeAreArgumentss()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            [DragonFruit2.Argument(1)]
            public required string Name { get; set; }
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Arguments);
        Assert.Empty(commandInfo.Options);
        Assert.Equal(1, commandInfo.Arguments.Single().Position);

    }

    [Fact]
    public async Task PropInfoHasName()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public int Name { get; set; }
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("Name", commandInfo.Options.Single().Name);

    }

    [Fact]
    public async Task PropInfoHasCliNameWithKebabCase()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public int GivenName { get; set; }
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("--given-name", commandInfo.Options.Single().CliName);

    }

    [Fact]
    public async Task PropInfoHasTypeName()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public int Name { get; set; }
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("int", commandInfo.Options.Single().TypeName);

    }

    [Fact]
    public async Task PropInfoRecognizesRequiredModifier()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public required string Name { get; set; }
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("Name", commandInfo.Options.Single().Name);
        Assert.True(commandInfo.Options.Single().HasRequiredModifier);

    }

    [Fact]
    public async Task PropInfoRecognizesLackOfRequiredModifier()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public string Name { get; set; }
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("Name", commandInfo.Options.Single().Name);
        Assert.False(commandInfo.Options.Single().HasRequiredModifier);

    }

    [Fact]
    public async Task PropInfoRecognizesInitializer()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public int Name { get; set; } = 42
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.True(commandInfo.Options.Single().HasInitializer);

    }

    [Fact]
    public async Task PropInfoHasInitializationText()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public int Name { get; set; } = 42
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("42", commandInfo.Options.Single().InitializerText);

    }

    [Fact]
    public async Task PropInfoHasValueTypeTrueForValueType()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public int Name { get; set; } = 42
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.True(commandInfo.Options.Single().IsValueType);

    }

    [Fact]
    public async Task PropInfoHasValueTypeFalseForReferenceType()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public string Name { get; set; } = 42
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.False(commandInfo.Options.Single().IsValueType);

    }

    [Fact]
    public async Task PropInfoHasNullableAnnotatedWhenPresent()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            public string? Name { get; set; } = 42
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal(NullableAnnotation.Annotated, commandInfo.Options.Single().NullableAnnotation);

    }

    [Fact]
    public async Task PropInfoHasFalseNullableNoneWhenAbsent()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass]  
        public partial class MyArgs
        { 
            public string Name { get; set; } = 42
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal(NullableAnnotation.None, commandInfo.Options.Single().NullableAnnotation);

    }

    [Fact]
    public async Task PropInfoHasDescriptionFromAttribute()
    {
        var sourceText = """
        using DragonFruit2;

        [CommandClass] 
        public partial class MyArgs
        { 
            [DragonFruit2.Description("R2D2")]
            public string Name { get; set; } = 42
        }
        """;

        var commandInfo = TestHelpers.CommandInfoFromSource(sourceText, TestHelpers.EmptyConsoleAppCode);

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal("R2D2", commandInfo.Options.Single().Description);

    }

    [Fact]
    public void CreatePropInfo_MultipleProperties_CreatesAllPropInfos()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
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


    [Fact]
    public void CreatePropInfo_SimpleStringProperty_CreatesValidPropInfo()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
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

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

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
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                public int Age { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Age");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.True(result.IsValueType);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithInitializer_CapturesInitializer()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                public string Greeting { get; set; } = "Hello";
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Greeting");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.True(result.HasInitializer);
        Assert.NotNull(result.InitializerText);
        Assert.Contains("Hello", result.InitializerText);
    }

    [Fact]
    public void CreatePropInfo_RequiredProperty_SetsHasRequiredModifier()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                public required string Name { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.True(result.HasRequiredModifier);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithArgumentAttribute_SetsIsArgument()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;
            
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                [Argument(Position = 0)]
                public string Name { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.True(result.HasArgumentAttribute);
        Assert.True(result.IsArgument);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithArgumentAttribute_CapturesPosition()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;
            
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                [Argument(5)]
                public string Name { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.Equal(5, result.Position);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithDescriptionAttribute_CapturesDescription()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;
            
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                [Description("User's full name")]
                public string Name { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.Equal("User's full name", result.Description);
    }

    [Fact]
    public void CreatePropInfo_NullableAnnotation_CapturesNullability()
    {
        var source = """
            namespace TestNamespace;
            #nullable enable
            
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                public string? Name { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Name");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.Equal(NullableAnnotation.Annotated, result.NullableAnnotation);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithoutInitializer_InitializerTextNull()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
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

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.False(result.HasInitializer);
        Assert.Null(result.InitializerText);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithoutArgumentAttribute_IsArgumentFalse()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
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

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.False(result.HasArgumentAttribute);
        Assert.False(result.IsArgument);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithoutDescriptionAttribute_DescriptionIsNull()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
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

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.Null(result.Description);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithNumericInitializer_CapturesInitializer()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                public int Count { get; set; } = 42;
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Count");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.True(result.HasInitializer);
        Assert.NotNull(result.InitializerText);
        Assert.Contains("42", result.InitializerText);
    }

    [Fact]
    public void CreatePropInfo_PropertyWithNotAnnotatedNullability_CapturesNullability()
    {
        var source = """
            namespace TestNamespace;
            #nullable enable
            
            using DragonFruit2;

            [CommandClass] 
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

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.Equal(NullableAnnotation.NotAnnotated, result.NullableAnnotation);
    }


    [Fact]
    public void CreatePropInfo_PropertyWithStringInitializerContainingSpecialChars_CapturesInitializer()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                public string Path { get; set; } = "C:\\Users\\Test";
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Path");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.True(result.HasInitializer);
        Assert.NotNull(result.InitializerText);
    }

    [Fact]
    public void CreatePropInfo_ReferenceTypeWithoutInitializer_IsValueTypeFalse()
    {
        var source = """
            namespace TestNamespace;
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                public object Data { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "Data");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.False(result.IsValueType);
    }

    [Fact]
    public void CreatePropInfo_NullableValueType_CapturesNullability()
    {
        var source = """
            namespace TestNamespace;
            #nullable enable
            
            using DragonFruit2;

            [CommandClass] 
            public class MyArgs
            {
                public int? OptionalAge { get; set; }
            }
            """;
        var argsTree = CSharpSyntaxTree.ParseText(source);
        var compilation = TestHelpers.GetCompilation(argsTree);
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.MyArgs");
        Assert.NotNull(typeSymbol);
        var propSymbol = typeSymbol.GetMembers().OfType<IPropertySymbol>().First(p => p.Name == "OptionalAge");
        Assert.NotNull(propSymbol);

        var result = PropInfoHelpers.CreatePropInfo(propSymbol, compilation.GetSemanticModel(argsTree));

        Assert.True(result.IsValueType);
        Assert.Equal(NullableAnnotation.Annotated, result.NullableAnnotation);
    }

}
