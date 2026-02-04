namespace DragonFruit2;

public interface ICreatesMembers<TReturn>
{
   TReturn CreateMember<TValue>(CommandDataDefinition commandDefinition, string name);
}
