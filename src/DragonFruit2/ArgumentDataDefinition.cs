namespace DragonFruit2;

public class ArgumentDataDefinition<TValue> : MemberDataDefinition<TValue>
{
    public ArgumentDataDefinition(CommandDataDefinition commandDefinition, string name) 
        : base(commandDefinition, name, false)
    {
    }

    public int Position { get; set; }
}
