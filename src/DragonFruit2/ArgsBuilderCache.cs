namespace DragonFruit2;

/// <summary>
/// Base class for each distinct CLI based on the root Args type. This allows
/// multiple CLIs to be defined in the same process.
/// </summary>
/// <remarks>
/// The need for this class is the design of having the command action set the active
/// ArgsBuilder, and by extension the active Args type. 
/// <br/>
/// This is not ideal, but it avoids static members in interfaces, which are not supported in .NET Standard 2.0.
/// <br/>
/// </remarks>
/// <typeparam name="TRootArgs"></typeparam>
public static class ArgsBuilderCache<TRootArgs>
    where TRootArgs : IArgs<TRootArgs>
{
    private static Dictionary<Type, ArgsBuilder<TRootArgs>> availableArgsBuilders = new();
    private static ArgsBuilder<TRootArgs>? activeArgsBuilder;

    public static ArgsBuilder<TRootArgs>? ActiveArgsBuilder
    {
        get => activeArgsBuilder;
        set => activeArgsBuilder = value;
    }
    internal static ArgsBuilder<TRootArgs>? GetActiveArgsBuilder() => ActiveArgsBuilder;

    public static void AddArgsBuilder<TArgs>(ArgsBuilder<TRootArgs> builder)
        where TArgs : IArgs<TArgs>
    {
        var type = typeof(TArgs);
        if (!typeof(ArgsBuilder<TRootArgs>).IsAssignableFrom(type))
        {
            throw new ArgumentException("The provided type is not a valid ArgsBuilder type.", nameof(type));
        }
        if (!availableArgsBuilders.ContainsKey(type))
        {
            availableArgsBuilders.Add(type, builder);
        }
    }

    public static ArgsBuilder<TArgs> GetArgsBuilder<TArgs>()
        where TArgs : IArgs<TArgs>
    {
        return (ArgsBuilder<TArgs>)GetArgsBuilder(typeof(TArgs));
    }

    private static ArgsBuilder GetArgsBuilder(Type type)
    {
        if (availableArgsBuilders.TryGetValue(type, out var builder))
        {
            return builder;
        }
        throw new ArgumentException("The args type requested has not been registered");
    }

}
