using DragonFruit2;
using DragonFruit2.Validators;

namespace DragondFruit2.Test.Project;

public class ValidatorTestsCli : EntryCli 
{
    [GreaterThan(0)]
    public int ShouldBeGreaterThanZero { get; init; };


}
