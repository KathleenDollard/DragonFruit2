using SampleConsoleApp;
using System.CommandLine.Parsing;

namespace SimpleConsoleApp;

/// <summary>
/// Test that run generation as part of build, and these tests are on that output
/// </summary>
/// <remarks>
/// The purpose of these tests is to ensure that the generated code runs correctly. 
/// The code is created _automatically on build_ via normal generation with a _project_ reference
/// to the generator. We might later consider the value of using a package reference.
/// <br/>
/// The generated code (that you will probably need to troubleshoot tests) is in 
/// "Dependencies/Analyzers/DragonFruit2.Generators/DragonFruit2.Generator.DragonFruit2Generator"
/// node of the solution exporer of this project in Visual Studio.
/// <br/>
/// Note that this does not call the 
/// </remarks>
public class IntegrationTests
{
    private TextWriter originalOutput = Console.Out;

    private void SetConsoleOut()
    {
        // Create a StringWriter to capture output
        var stringWriter = new StringWriter();
        // Save the original Console.Out

        // Redirect Console.Out to the StringWriter
        Console.SetOut(stringWriter);
    }

    private void ResetConsoleOut()
    {
        // Create a StringWriter to capture output

        // Restore the original Console.Out in the finally block
        Console.SetOut(originalOutput);
    }

    [Theory]
    [ClassData(typeof(IntegrationTheoryData))]
    public void String_options_are_retrieved(string _, string cliInput, string expectedName, int __, string ___)
    {

        var result = global::Cli.ParseArgs<MyArgs>(CommandLineParser.SplitCommandLine(cliInput).ToArray());

        Assert.True(result.IsValid);
        Assert.Equal(expectedName, result.Args?.Name);
    }

    [Theory]
    [ClassData(typeof(IntegrationTheoryData))]
    public void Int_options_are_retrieved(string _, string cliInput, string __, int expectedAge, string ___)
    {
        var result = global::Cli.ParseArgs<MyArgs>(CommandLineParser.SplitCommandLine(cliInput).ToArray());

        Assert.True(result.IsValid);
        Assert.Equal(expectedAge, result.Args?.Age);
    }

    [Theory]
    [ClassData(typeof(IntegrationTheoryData))]
    public void Default_values_are_applied(string _, string cliInput, string __, int ___, string greeting)
    {
        var result = global::Cli.ParseArgs<MyArgs>(CommandLineParser.SplitCommandLine(cliInput).ToArray());

        Assert.True(result.IsValid);
        Assert.Equal(greeting, result.Args?.Greeting);
    }
}
