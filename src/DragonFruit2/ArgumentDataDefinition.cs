namespace DragonFruit2;

public class ArgumentDataDefinition : MemberDataDefinition
{
    public ArgumentDataDefinition(Type argsType, string name) 
        : base(argsType, name)
    {
    }

    public int Position { get; set; }
}