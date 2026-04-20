using DragonFruit2.Validators;

namespace DragonFruit2.Test.Functional;

public partial class ValidatorTestsCommand : CommandRootBase<ValidatorTestsCommand>, IReportSelf
{
    // Properties in this Command should not be required. Use another command,
    // possibly a subcommand of this for organization.
    //
    // Otherwise, everyone writing a test will have to include your property
    // in their commandline, and adding a required property will break existing
    // tests.
    //
    [GreaterThan(0)]
    public int ShouldBeGreaterThanZero { get; init; }

    public void Report()
    {
        throw new NotImplementedException();
    }
}
