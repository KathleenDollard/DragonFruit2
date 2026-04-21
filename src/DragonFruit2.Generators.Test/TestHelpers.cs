using DragonFruit2.Generators.Metadata;
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

    public const string ConsoleAppWithTwoDifferentCalls = """
        using DragonFruit2;
        
        var myArgs = Cli.ParseArgs<MyArgs>(args);
        var myArgs2 = Cli.ParseArgs<MyOtherArgs>(args);
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

    public static (string?, IEnumerable<Microsoft.CodeAnalysis.Diagnostic>) GetGeneratorDriver(params string[] sources)
    {
        var compilation = GetCompilation(sources);

        var generator = new DragonFruit2Generator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        return (outputCompilation.SyntaxTrees.LastOrDefault()?.ToString(), diagnostics);
    }

    public static Compilation GetCompilation(params IEnumerable<string> sources)
    {
        var syntaxTrees = sources.Select(x => CSharpSyntaxTree.ParseText(x)).ToArray();
        return GetCompilation(syntaxTrees);
    }

    public static Compilation GetCompilation(params SyntaxTree[] syntaxTrees)
    {
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

    internal static IEnumerable<CommandInfo?> GetCommandInfos(string source, string appSource)
    {
        var compilation = GetCompilation(source, appSource);
        var syntaxTree = compilation.SyntaxTrees.First();
        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
        var classes = GetCommandClasses(syntaxTree, semanticModel);
        return classes
            .Select(c => CommandBuilder.GetCommandInfo(c, semanticModel))
            .Where(i => i is not null)
            .ToList();
    }

    internal static CommandInfo? GetCommandInfo(string source, string appSource)
    {
        var commandInfos = GetCommandInfos(source, appSource).ToList();
        return commandInfos switch
        {
            [] => null,
            [_] => commandInfos.Single(),
            _ => throw new InvalidOperationException("Multiple CommandInfos retrieved")
        };
    }



    //public static IEnumerable<InvocationExpressionSyntax> GetParseArgsInvocations(SyntaxTree syntaxTree)
    //    => [.. syntaxTree.GetRoot()
    //        .DescendantNodes()
    //        .OfType<InvocationExpressionSyntax>()
    //        .Where(invocation =>
    //            invocation.Expression switch
    //            {
    //                MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
    //                    => DragonFruit2Builder.IsMethodNameOfInterest(gns.Identifier.ValueText) && gns.TypeArgumentList.Arguments.Count == 1,
    //                GenericNameSyntax gns2
    //                    => DragonFruit2Builder.IsMethodNameOfInterest(gns2.Identifier.ValueText) && gns2.TypeArgumentList.Arguments.Count == 1,
    //                _ => false,
    //            })];

    public static IEnumerable<ClassDeclarationSyntax> GetCommandClasses(SyntaxTree syntaxTree, SemanticModel semanticModel)
    {
        return [.. syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Where(c=>HasCommandClassAttribute(c, semanticModel))];

        static bool HasCommandClassAttribute(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (symbol is null)
            {
                return false;
            }
            var attributes = symbol.GetAttributes();
            var match = "DragonFruit2.CommandClassAttribute";
            return attributes.Any(a => a.AttributeClass?.ToDisplayString() == match);
        }
    }

    internal static IEnumerable<CommandNode> GetCommandNodeInfos(string argsSource, string consoleSource)
    {
        var commandInfos = GetCommandInfos(argsSource, consoleSource);
        return CommandBuilder.BuildCommandTree(commandInfos!);

    }

    internal static IEnumerable<CliInfo?> GetCliInfos(Compilation compilation)
    {
        return compilation.SyntaxTrees
                .SelectMany(tree => CliInfoFromSyntaxTree(compilation, tree));

        static IEnumerable<CliInfo> CliInfoFromSyntaxTree(Compilation compilation, SyntaxTree syntaxTree)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocations = GetCliInvocations(syntaxTree, semanticModel);
            var cliInfos =invocations
                .Select(i => CliBuilder.GetCliInfo(i, semanticModel))
                .ToList();
            return cliInfos
                .Where(i => i is not null)!;

        }
    }

    internal static IEnumerable<InvocationExpressionSyntax> GetCliInvocations(SyntaxTree syntaxTree, SemanticModel semanticModel)
    {
        return [..syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(invocation =>
                    invocation.Expression switch
                    {
                        MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                            => !(CliBuilder.IsMethodNameOfInterest(gns.Identifier.ValueText) || gns.TypeArgumentList.Arguments.Count != 1),
                        GenericNameSyntax gns2
                            => CliBuilder.IsMethodNameOfInterest(gns2.Identifier.ValueText) && gns2.TypeArgumentList.Arguments.Count == 1,
                        _ => false,
                    })];


    }
}

