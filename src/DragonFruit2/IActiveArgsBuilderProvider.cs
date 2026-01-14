using DragonFruit2.Validators;

namespace DragonFruit2;

public interface IActiveArgsBuilderProvider<TRootArgs>
    where TRootArgs : class, IArgs<TRootArgs>
{
    (IEnumerable<ValidationFailure>? failures, ArgsBuilder<TRootArgs>? builder) GetActiveArgsBuilder();
}
