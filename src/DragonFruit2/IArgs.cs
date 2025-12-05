using DragonFruit2.Common.Net8;

namespace DragonFruit2.Common;

public interface IArgs<TArgs>
    where TArgs :IArgs<TArgs>
{
    public abstract static void Initialize(Runner<TArgs> runner);
    public abstract static TArgs Create(Runner<TArgs> runner);

}
