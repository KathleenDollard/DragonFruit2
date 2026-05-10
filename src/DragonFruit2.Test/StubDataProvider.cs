namespace DragonFruit2.Test;

/// <summary>
/// This is a stub generator for use during testing, such as validator testing, that 
/// requires calling DataValue.SetValue
/// </summary>
internal class StubDataProvider : DataProvider<StubRootCommand>
{
    public static DataProvider Instance => new StubDataProvider(new Builder<StubRootCommand>(null!));
    public StubDataProvider(Builder<StubRootCommand> builder) : base(builder)
    {
    }
    protected override bool TryGetValue<TValue>(MemberDataDefinition<TValue> memberDefinition,
                                             Result<StubRootCommand> result,
                                             out TValue Value)
    {
        throw new NotImplementedException();
    }
}


public class StubRootCommand  { }