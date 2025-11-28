using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace DragonFruit2.Generators.Test;

public class GenerationTests
{
    [Fact]
    public async Task CanOutputGenerationAsync()
    {
        var context = new CSharpSourceGeneratorTest<DragonFruit2Generator, DefaultVerifier>();
        context.ReferenceAssemblies = ReferenceAssemblies.Net.Net90;
        context.TestCode = """
            using DragonFruit2;

            [DragonFruit2.Cli] 
            public class Dummy { }

            [DragonFruit2.Cli] public class Dummy2 { }

            class Dummy3{ }
            """;
            
        context.TestState.GeneratedSources.Add((typeof(DragonFruit2Generator), "ParseArgsGenerator_CollectedTypes.g.cs",
            @"// Hello World"));

     await context.RunAsync();

    }
}
