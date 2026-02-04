namespace DragonFruit2;

internal interface IActiveArgsProvider<TRootArgs>
        where TRootArgs : ArgsRootBase<TRootArgs>
{
    bool TryGetActiveArgsDefinition(out CommandDataDefinition<TRootArgs> args);
}