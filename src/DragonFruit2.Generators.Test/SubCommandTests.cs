using DragonFruit2.Generators;
using DragonFruit2.Generators.Test;

namespace DragonFruit2.Validators;


public class SubCommandTests
{
    [Fact]
    public void SubCommandCanBeCreated()
    {
        var sourceText = """
            public partial class MyArgs
            {
                public required string Name { get; set; }
            }

            public partial class MorningGreetingArgs : MyArgs
            {
            }

            public partial class EveningGreetingArgs : MyArgs
            {
                public int Age { get; init; } = 1;
            }
            """;
        var compilation = TestHelpers.GetCompilation(sourceText, TestHelpers.EmptyConsoleAppCode);
        var programTree = compilation.SyntaxTrees.Last();
        var invocations = TestHelpers.GetParseArgsInvocations(programTree);
        Assert.Single(invocations);

        var commandInfo = DragonFruit2Builder.GetRootCommandInfoFromInvocation(invocations.Single(), compilation.GetSemanticModel(programTree));

        Assert.NotNull(commandInfo);
        Assert.Single(commandInfo.Options);
        Assert.Equal(2, commandInfo.SubCommands.Count);
        var subMorning = commandInfo.SubCommands[0];
        Assert.NotNull(subMorning);
        Assert.Empty(subMorning.Options);
        var subEvening = commandInfo.SubCommands[1];
        Assert.NotNull(subEvening);
        Assert.Single(subEvening.Options);


    }
}
