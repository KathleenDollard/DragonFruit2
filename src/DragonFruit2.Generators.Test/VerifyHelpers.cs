using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.CompilerServices;

namespace DragonFruit2.Generators.Test;

public static class VerifyHelpers
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }

    public static GeneratorDriver GetDriver(string source, string consoleSource)
    {
        var compilation = TestHelpers.GetCompilation(source, consoleSource);

        DragonFruit2Generator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        return driver;
    }
}