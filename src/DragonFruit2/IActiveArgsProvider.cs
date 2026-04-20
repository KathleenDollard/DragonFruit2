using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

internal interface IActiveArgsProvider<TRootArgs>
        where TRootArgs : CommandRootBase<TRootArgs>
{
    bool TryGetActiveArgsDefinition(Result<TRootArgs> result,
                                    [NotNullWhen(true)] out CommandDataDefinition<TRootArgs> activeCommandDefition,
                                    [NotNullWhen(true)] out DataProvider<TRootArgs> activeDataProvider);
}