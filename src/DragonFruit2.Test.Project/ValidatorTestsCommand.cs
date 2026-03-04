using DragonFruit2;
using DragonFruit2.Test.Project;
using DragonFruit2.Validators;

namespace DragondFruit2.Test.Project;

public partial class ValidatorTestsCommand : ArgsRootBase<ValidatorTestsCommand>, IReportSelf
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
