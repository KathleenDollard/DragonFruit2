using Argon;
using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.Test;
#pragma warning disable xUnit1026 // Theory is reused in several tests that use different properties/parameters

public class VerifyTests
{
    private static readonly string[] _expectedDragonFruit2TrackingNames = [
                        "DragonFruit2.ExtractEntryPoint",
                        "DragonFruit2.ExtractCommandClasses",
                        "DragonFruit2.BuildCommandNodes",
                        "DragonFruit2.CliInfoGroups",
                        ];

    private VerifySettings VerifySettings(string directory)
    {
        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory($"Snapshots/{directory}");
        verifySettings.DontIgnoreEmptyCollections();
        verifySettings.AddExtraSettings(s => s.Converters.Add(new VerifyCommandNodeEnumerableSerializer()));
        verifySettings.AddExtraSettings(s => s.NullValueHandling = NullValueHandling.Include);
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
        var cliInfoGroup = CommandBuilder.GetCliInfoGroups(cliInfos, new CancellationToken());

        return Verify(cliInfoGroup, VerifySettings("CliInfoGroup")).UseParameters(desc);
    }

    /// <summary>
    /// </summary>
    /// <param name="desc">Used as part of file name, so keep it short</param>
    /// <param name="argsSource">The source code for the command classes</param>
    /// <param name="consoleSource">The source code for the console application with the CLI entry point call</param>
    /// <returns></returns>
    [Theory]
    [ClassData(typeof(CommandInfoTheoryData))]
    public Task SourceToSource(string desc, string argsSource, string consoleSource)
    {
        var compilation = TestHelpers.GetCompilation(argsSource, consoleSource);
        var driver = VerifyHelpers.GetGeneratorDriver<DragonFruit2Generator>(compilation);

        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory("Snapshots/GenOutput");
        return Verify(driver, verifySettings).UseParameters(desc);
    }

    /// <summary>
    /// </summary>
    /// <param name="desc">Used as part of file name, so keep it short</param>
    /// <param name="argsSource">The source code for the command classes</param>
    /// <param name="consoleSource">The source code for the console application with the CLI entry point call</param>
    /// <returns></returns>
    [Theory]
    [ClassData(typeof(CommandInfoTheoryData))]
    public void ConfirmCaching(string desc, string argsSource, string consoleSource)
    {
        var compilation = TestHelpers.GetCompilation(argsSource, consoleSource);
        var clonedCompilation = compilation.Clone();
       
        var firstRunResult = VerifyHelpers.GetGeneratorDriver<DragonFruit2Generator>(compilation).GetRunResult();
        var secondRunResult = VerifyHelpers.GetGeneratorDriver<DragonFruit2Generator>(clonedCompilation).GetRunResult();

        VerifyHelpers.AssertRunResultsEqual(firstRunResult, secondRunResult, _expectedDragonFruit2TrackingNames);

    }



}
