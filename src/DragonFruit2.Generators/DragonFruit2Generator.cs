using DragonFruit2.GeneratorSupport;
using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators
{
    [Generator]
    public sealed partial class DragonFruit2Generator : IIncrementalGenerator
    {
        private static readonly DragonFruit2Builder builder = new();

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var rawCommandInfos = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => builder.InitialFilter (node),
                    transform: static (ctx, _) => builder.Transform(ctx))
                .WithTrackingName("ParseArgsInvocations")
                .Where(static s => s is not null)
                .Select(static (s, _) => s!) // Quiet nullability warning
                .Collect();

            var commandInfos = rawCommandInfos.Select((infos, ctx) => builder.BindParents(infos));

            var allCommandInfos = commandInfos
                .SelectMany(static (collected, _) => collected.SelectMany(commandInfo => builder.GetSelfAndDescendants(commandInfo)))
                .WithTrackingName("AllCommandInfos");

            var collectedAllCommandInfos = allCommandInfos.Collect();

            context.RegisterSourceOutput(collectedAllCommandInfos,
                    static (spc, cmdInfos) =>
                    {
                        spc.AddSource("DebuggingInfo", "//" + string.Join(", ", cmdInfos.Select(ci => ci.Name)));
                    });

            context.RegisterSourceOutput(allCommandInfos, 
                static (spc, collected) =>
            {
                builder.OutputSource(spc, collected);
            });
        }

    }

}
