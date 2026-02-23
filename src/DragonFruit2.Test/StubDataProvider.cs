namespace DragonFruit2.Test;

/// <summary>
/// This is a stub generator for use during testing, such as validator testing, that 
/// requires calling DataValue.SetValue
/// </summary>
internal class StubDataProvider : DataProvider<StubRootArgs>
{
    public static DataProvider Instance => new StubDataProvider(new Builder<StubRootArgs>(null!));
    public StubDataProvider(Builder<StubRootArgs> builder) : base(builder)
    {
    }
    public override bool TryGetValue<TValue>(MemberDataDefinition<TValue> memberDefinition,
                                             Result<StubRootArgs> result,
                                             out TValue Value)
    {
        throw new NotImplementedException();
    }
}


public class StubRootArgs : ArgsRootBase<StubRootArgs> { }