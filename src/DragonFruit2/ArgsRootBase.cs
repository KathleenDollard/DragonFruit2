namespace DragonFruit2;

public abstract class CommandRootBase
{
    public virtual bool Initialize()
        => true;

}

public abstract class CommandRootBase<TArgs> : CommandRootBase
    where TArgs : CommandRootBase<TArgs>
{ }
