using DragonFruit2.Validators;

namespace DragonFruit2;

public abstract class ArgsRootBase<TArgs>
    where TArgs : ArgsRootBase<TArgs>
{
    public virtual IEnumerable<ValidationFailure> Validate()
    {
        return Enumerable.Empty<ValidationFailure>();
    }

}
