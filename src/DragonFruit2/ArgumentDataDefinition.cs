namespace DragonFruit2;

public class ArgumentDataDefinition<TValue> : MemberDataDefinition<TValue>
{
    public ArgumentDataDefinition(Type argsType, string name) 
        : base(argsType, name, false)
    {
    }

    public int Position { get; set; }
}
