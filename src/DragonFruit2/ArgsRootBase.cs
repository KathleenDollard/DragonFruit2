namespace DragonFruit2;

public abstract class ArgsRootBase
{
    public virtual IEnumerable<Diagnostic> Validate()
    {
        return Enumerable.Empty<Diagnostic>();
    }

    public virtual bool Initialize()
        => true;

}

public abstract class ArgsRootBase<TArgs> : ArgsRootBase
    where TArgs : ArgsRootBase<TArgs>
{ }
