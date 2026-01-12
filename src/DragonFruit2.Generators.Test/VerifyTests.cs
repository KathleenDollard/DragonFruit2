using DragonFruit2.GeneratorSupport;
using VerifyTests;

namespace DragonFruit2.Generators.Test;

public class VerifyTests
{


    //private readonly VerifySettings _verifySettings = new();
    //public VerifyTests()
    //{
    //    _verifySettings.UseDirectory("Snapshots");
    //    // DiffTools.UseOrder(DiffTool.MsWordDiff);
    //}

    [Fact]
    public Task VerifyCheck() =>
        VerifyChecks.Run();

    [Theory]
    [ClassData(typeof(CommandInfoTheoryData))]
    public Task CommandInfo(string desc, string argsSource, string consoleSource, CommandInfo expected, string _)
    {
        var compilation = TestHelpers.GetCompilation(argsSource, consoleSource);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        var actual = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));
        
        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory("Snapshots/CommandInfo");
        return Verify(actual, verifySettings).UseParameters(desc);
    }

    [Theory]
    [ClassData(typeof(CommandInfoTheoryData))]
    public Task CommandOutput(string desc, string _, string __, CommandInfo commandInfo, string expected)
    {
        var actual = DragonFruit2Builder.GetSourceForCommandInfo(commandInfo);

        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory("Snapshots/CommandInfoOutput");
        return Verify(actual, verifySettings).UseParameters(desc);
    }

    [Theory]
    [ClassData(typeof(CommandInfoTheoryData))]
    public Task Generation(string desc, string argsSource, string consoleSource, CommandInfo _, string __)
    {
        var driver = VerifyHelpers.GetDriver(argsSource, consoleSource);

        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory("Snapshots/GenOutput");
        return Verify(driver, verifySettings).UseParameters(desc);
    }
}
