namespace DragonFruit2;

public interface IArgs<TArgs>
    where TArgs :IArgs<TArgs>
{
    public abstract static void Initialize(Builder<TArgs> builder);
    public abstract static TArgs Create(Builder<TArgs> builder);

}
