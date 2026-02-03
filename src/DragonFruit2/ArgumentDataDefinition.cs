namespace DragonFruit2;

public class ArgumentDataDefinition : MemberDataDefinition
{
    public ArgumentDataDefinition(string fullName) 
        : base(fullName)
    {
    }

    public int Position { get; set; }
}