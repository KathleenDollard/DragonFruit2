using DragonFruit2.GeneratorSupport;
using VerifyTests;

namespace DragonFruit2.Generators.Test;

public class VerifyTests
{

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

    /// <summary>
    /// This test returns the output for teh root args only. The integration test returns all of the files
    /// </summary>
    /// <param name="desc">Used as part of file name, so keep it short</param>
    /// <param name="_">Not used</param>
    /// <param name="__">Not used</param>
    /// <param name="commandInfo">The CommandInfo to use for generation (from theory)</param>
    /// <param name="___">Not used</param>
    /// <returns></returns>
    [Theory]
    [ClassData(typeof(CommandInfoTheoryData))]
    public Task CommandOutput(string desc, string _, string __, CommandInfo commandInfo, string ___)
    {
        var actual = DragonFruit2Builder.GetSourceForCommandInfo(commandInfo);

        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory("Snapshots/CommandOutput");
        return Verify(actual, verifySettings).UseParameters(desc);
    }




    // The following test should ideally generate new code and compare to the code in the CommandOutput test 
    //[Theory]
    //[ClassData(typeof(CommandInfoTheoryData))]
    //public Task Generation(string desc, string argsSource, string consoleSource, CommandInfo _, string __)
    //{
    //    var driver = VerifyHelpers.GetDriver(argsSource, consoleSource);

    //    var verifySettings = new VerifySettings();
    //    verifySettings.UseDirectory("Snapshots/GenOutput");
    //    return Verify(driver, verifySettings).UseParameters(desc);
    //}
}
