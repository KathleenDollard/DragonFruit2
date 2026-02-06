namespace DragonFruit2;

public interface ICreatesFromMembers<TReturn>
{
   TReturn CreateFromMember<TValue>(CommandDataDefinition commandDefinition, string name);
}
