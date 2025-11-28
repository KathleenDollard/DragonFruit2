using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace DragonFruit2.Generators.Test;

public static class TestHelpers
{
    public const string  EmptyConsoleAppCode = """
        using DragonFruit2;
        
        var myArgs = Command.ParseArgs<MyArgs>(args);
        """;

    public const string EmptyConsoleAppCodeWithArgsMyNamespace = """
        using DragonFruit2;
        using MyNamespace;
        
        var myArgs = Command.ParseArgs<MyArgs>(args);
        """;

    public static GeneratorDriver GetGeneratorDriver(params string[] sources)
    {
        var syntaxTrees = sources.Select(x => CSharpSyntaxTree.ParseText(x)).ToArray();

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: syntaxTrees);

        var generator = new DragonFruit2Generator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        return driver.RunGenerators(compilation);

    }

    public static Compilation GetCompilation(params string[] sources)
    {
        var syntaxTrees = sources.Select(x => CSharpSyntaxTree.ParseText(x)).ToArray();
        //List<MetadataReference> references =  [MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        //                                       MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)];
        //references.Add(MetadataReference.Cr(ReferenceAssemblies.NetStandard.NetStandard20));
        //references.Add(MetadataReference.CreateFromFile(typeof(DragonFruit2.ArgumentAttribute).Assembly.Location));
        //references.Add(MetadataReference.CreateFromFile(typeof(System.Attribute).Assembly.Location));
        //references.Add(MetadataReference.CreateFromFile(typeof(System.Object).Assembly.Location));

        var references = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(assembly => !assembly.IsDynamic)
                            .Select(assembly => MetadataReference
                                                .CreateFromFile(assembly.Location))
                            .Cast<MetadataReference>()
                            .ToList();
        references.Add(MetadataReference.CreateFromFile(@"..\..\DragonFruit2\debug\DragonFruit2.dll"));

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: syntaxTrees,
            references: references);

        return compilation;
    }

    public static IEnumerable<InvocationExpressionSyntax> GetParseArgsInvocations(SyntaxTree syntaxTree) 
        => syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(invocation =>
                invocation.Expression switch
                {
                    MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                        => gns.Identifier.ValueText == "ParseArgs" && gns.TypeArgumentList.Arguments.Count == 1,
                    GenericNameSyntax gns2
                        => gns2.Identifier.ValueText == "ParseArgs" && gns2.TypeArgumentList.Arguments.Count == 1,
                    _ => false,
                })
            .ToList();


    //public static IEnumerable<GenericNameSyntax> GetParseArgGenericNameSyntax(Compilation compilation)
    //{
    //    var program = compilation.GlobalNamespace.GetTypeMembers()
    //        .Where(member => member is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.Name == "Program")
    //        .FirstOrDefault()
    //        ?? throw new InvalidOperationException("Could not find Program class in compilation");

    //    var invocations = program.GetMembers()
    //        .OfType<IMethodSymbol>()
    //        .SelectMany(method => method.DeclaringSyntaxReferences)
    //        .SelectMany(reference => reference.GetSyntax().DescendantNodes())
    //        .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax>()
    //        .ToList();

    //    var genericNameSyntaxes = invocations.Select(invocation => invocation.Expression)
    //        .Select(GetParseArgsGenericNameSyntax)
    //        .Where(gns => gns is not null)
    //        .ToList();

    //    return genericNameSyntaxes!; // Nullable reference types doesn't catch the LINQ null filtering here

    //    static GenericNameSyntax? GetParseArgsGenericNameSyntax(SyntaxNode node)
    //    {
    //        return node switch
    //        {
    //            MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns && gns.Identifier.ValueText == "ParseArgs" => gns,
    //            GenericNameSyntax gns2 => gns2,
    //            _ => null,
    //        };
    //    }
    //}
}

