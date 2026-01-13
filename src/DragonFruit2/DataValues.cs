using DragonFruit2.Validators;

namespace DragonFruit2;

public class DataValues<TArgs>
    where TArgs : IArgs<TArgs>
{
    public List<ValidationFailure> ValidationFailures = [];
    public bool IsValid => !ValidationFailures.Any();
    public TArgs? Args { get; set; }
}
