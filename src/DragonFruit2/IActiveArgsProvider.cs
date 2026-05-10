using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2;

internal interface IActiveCommandProvider<TRootCommand> 
{
    bool TryGetActiveCommandDefinition(Result<TRootCommand> result,
                                    [NotNullWhen(true)] out CommandDataDefinition<TRootCommand> activeCommandDefition,
                                    [NotNullWhen(true)] out DataProvider<TRootCommand> activeDataProvider);
}