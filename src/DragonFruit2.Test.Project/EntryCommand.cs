using DragonFruit2;

namespace DragonFruit2.Test.Project;

public partial class EntryCommand : ArgsRootBase<EntryCommand>
{
    // This will benefit from enum or choice support, but for now, sorry, you gotta' type it out.
    public required string Command { get; init; }
}
