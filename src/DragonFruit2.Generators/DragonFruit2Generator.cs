using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators
{
    [Generator]
    public sealed partial class DragonFruit2Generator : IIncrementalGenerator
    {
        private static readonly DragonFruit2Builder builder = new();

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var parseArgsInvocations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => builder.InitialFilter (node),
                    transform: static (ctx, _) => builder.Transform(ctx))
                .WithTrackingName("ParseArgsInvocations")
                .Where(static s => s is not null)
                .Select(static (s, _) => s!) // Quiet nullability warning
                .Collect();

            context.RegisterSourceOutput(parseArgsInvocations, static (spc, collected) =>
            {
                builder.OutputSource(spc, collected);
            });
        }

    }

}
