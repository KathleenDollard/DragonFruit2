using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.Test;
#pragma warning disable xUnit1026 // Theory is reused in several tests that use different properties/parameters

public class VerifyTests
{
    private VerifySettings VerifySettings(string directory) 
        {
            var verifySettings = new VerifySettings();
            verifySettings.UseDirectory($"Snapshots/{directory}");
            verifySettings.DontIgnoreEmptyCollections();
            verifySettings.AddExtraSettings(s => s.Converters.Add(new VerifyCommandNodeEnumerableSerializer()));
            return verifySettings;
        } 

    [Fact]
    public Task VerifyCheck() =>
        VerifyChecks.Run();

    [Theory]
    [ClassData(typeof(CommandInfoTheoryData))]
    public Task VerifyCommandInfo(string desc, string argsSource, string consoleSource)
    {
        var commandInfos = TestHelpers.GetCommandInfos(argsSource, consoleSource);

        var verifySettings = new VerifySettings();
        return Verify(commandInfos, VerifySettings("CommandInfo")).UseParameters(desc);
    }


    [Theory]
    [ClassData(typeof(CommandInfoTheoryData))]
    public Task VerifyCommandNode(string desc, string argsSource, string consoleSource)
    {
        var commandNodes = TestHelpers.GetCommandNodeInfos(argsSource, consoleSource);

        return Verify(commandNodes, VerifySettings("CommandNode")).UseParameters(desc);
    }

    [Theory]
    [ClassData(typeof(CliInfoTheoryData))]
    public Task VerifyCliInfo(string desc, IEnumerable<string> consoleSources)
    {
        var compilation = TestHelpers.GetCompilation(consoleSources);
        var cliInfos = TestHelpers.GetCliInfos(compilation);

        return Verify(cliInfos, VerifySettings("CliInfo")).UseParameters(desc);
    }


    [Theory]
    [ClassData(typeof(CliInfoTheoryData))]
    public Task VerifyCliInfoGroup(string desc, IEnumerable<string> consoleSources)
    {
        var compilation = TestHelpers.GetCompilation(consoleSources);
        var cliInfos = TestHelpers.GetCliInfos(compilation);
        var cliInfoGroup = CommandBuilder.GetCliInfoGroups(cliInfos, new System.Threading.CancellationToken());

        return Verify(cliInfoGroup, VerifySettings("CliInfoGroup")).UseParameters(desc);
    }

    /// <summary>
    /// </summary>
    /// <param name="desc">Used as part of file name, so keep it short</param>
    /// <param name="_">Not used</param>
    /// <param name="__">Not used</param>
    /// <param name="commandInfo">The CommandInfo to use for generation (from theory)</param>
    /// <param name="___">Not used</param>
    /// <returns></returns>
    [Theory]
    [ClassData(typeof(CommandInfoTheoryData))]
    public Task SourceToSource(string desc, string argsSource, string consoleSource)
    {
        var driver = VerifyHelpers.GetDriver(argsSource, consoleSource);

        var runResult = driver.GetRunResult();

        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory("Snapshots/GenOutput");
        return Verify(driver, verifySettings).UseParameters(desc);
    }
}
