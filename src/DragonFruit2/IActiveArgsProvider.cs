namespace DragonFruit2;

internal interface IActiveArgsProvider<TRootArgs>
        where TRootArgs : ArgsRootBase<TRootArgs>
{
    bool TryGetActiveArgsDefinition(Result<TRootArgs> result, out CommandDataDefinition<TRootArgs> activeCommandDefition);
}