using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators;

public abstract class GeneratorBuilder<T>
{
    public abstract bool InitialFilter(SyntaxNode node);
    public abstract T? Transform(GeneratorSyntaxContext context);
    public abstract void OutputSource(SourceProductionContext context, T item);
    //public abstract void OutputSource(SourceProductionContext context, IEnumerable<T> items);


}
