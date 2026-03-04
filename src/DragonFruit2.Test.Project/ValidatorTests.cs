using DragondFruit2.Test.Project;
using DragonFruit2.Validators;

namespace DragonFruit2.Test.Project;

public class ValidatorTests
{
    [Fact]
    public void GreaterThanAttributeResultsInFailureOnBadValues()
    {
        var commandLine = "--command -1";

        var result = Cli.ParseArgs<ValidatorTestsCommand>([commandLine]);

        Assert.Single(result.Diagnostics);
        Assert.Equal(DiagnosticId.GreaterThan.ToValidationIdString(), result.Diagnostics.First().Id);
    }
}
