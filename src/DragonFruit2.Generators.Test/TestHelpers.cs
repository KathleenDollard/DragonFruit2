using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace DragonFruit2.Generators.Test;

public static class TestHelpers
{
    public const string EmptyConsoleAppCode = """
        using DragonFruit2;
        
        var myArgs = Cli.ParseArgs<MyArgs>(args);
        """;

    public const string EmptyConsoleAppCodeWithArgsMyNamespace = """
        using DragonFruit2;
        using MyNamespace;
        
        var myArgs = Cli.ParseArgs<MyArgs>(args);
        """;

    public const string ConsoleAppWithDuplicateCall = """
        using DragonFruit2;
        
        var myArgs = Cli.ParseArgs<MyArgs>(args);
        var myArgs2 = Cli.ParseArgs<MyArgs>(args);
        """;

    public const string ConsoleAppWithTryParseCall = """
        using DragonFruit2;
        
        var success = Cli.TryParseArgs<MyArgs>(args, out var result);
        if (success)
        {
            // do something
        }
        """;

    public const string ConsoleAppWithTryExecuteCall = """
        using DragonFruit2;
        
        if (Cli.TryExecute<MyArgs>(args, out var result))
        {
            // do something
        }
        """;

    public static (string?, IEnumerable<Diagnostic>) GetGeneratorDriver(params string[] sources)
    {
        var compilation = GetCompilation(sources);

        var generator = new DragonFruit2Generator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        return (outputCompilation.SyntaxTrees.LastOrDefault()?.ToString(), diagnostics);
    }

    public static Compilation GetCompilation(params string[] sources)
    {
        var syntaxTrees = sources.Select(x => CSharpSyntaxTree.ParseText(x)).ToArray();
        var references = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(assembly => !assembly.IsDynamic)
                            .Select(assembly => MetadataReference
                                                .CreateFromFile(assembly.Location))
                            .Cast<MetadataReference>()
                            .ToList();
        references.Add(MetadataReference.CreateFromFile(@"..\..\DragonFruit2\debug\DragonFruit2.dll"));
        references.Add(MetadataReference.CreateFromFile(@"..\..\DragonFruit2.Common\debug\DragonFruit2.Common.dll"));

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: syntaxTrees,
            references: references);

        return compilation;
    }

    public static IEnumerable<InvocationExpressionSyntax> GetParseArgsInvocations(SyntaxTree syntaxTree)
        => [.. syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(invocation =>
                invocation.Expression switch
                {
                    MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                        => DragonFruit2Builder.IsMethodNameOfInterest(gns.Identifier.ValueText) && gns.TypeArgumentList.Arguments.Count == 1,
                    GenericNameSyntax gns2
                        => DragonFruit2Builder.IsMethodNameOfInterest(gns2.Identifier.ValueText) && gns2.TypeArgumentList.Arguments.Count == 1,
                    _ => false,
                })];


}

