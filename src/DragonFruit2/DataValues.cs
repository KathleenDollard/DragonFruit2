using DragonFruit2;

namespace DragonFruit2;

public abstract class DataValues
{ 
}

public abstract class DataValues<TRootArgs> : DataValues
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    public abstract void SetDataValues(DataProvider<TRootArgs> dataProvider);

    protected internal abstract TRootArgs CreateInstance();
}
