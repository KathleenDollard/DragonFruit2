namespace DragonFruit2.Test.Functional;

public partial class EntryCommand : CommandRootBase<EntryCommand>
{
    // This will benefit from enum or choice support, but for now, sorry, you gotta' type it out.
    public required string Command { get; init; }
}
