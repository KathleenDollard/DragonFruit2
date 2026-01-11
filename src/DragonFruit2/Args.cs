using DragonFruit2.Validators;

namespace DragonFruit2;

public abstract partial class Args<TArgs>
    where TArgs : Args<TArgs>
{
    public abstract IEnumerable<ValidationFailure> Validate();
}
