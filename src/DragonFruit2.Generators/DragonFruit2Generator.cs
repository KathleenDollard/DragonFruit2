using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators;

[Generator]
public sealed partial class DragonFruit2Generator : IIncrementalGenerator
{
    private static readonly DragonFruit2Builder builder = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var rootCommandInfos = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => builder.InitialFilter(node),
                transform: static (ctx, _) => builder.Transform(ctx))
            .WithTrackingName("ParseArgsInvocations")
            .Where(static s => s is not null)
            .Select(static (s, _) => s!) // Quiet nullability warning
            .Collect()
            .WithTrackingName(TrackingNames.Extract);

        var boundRootCommandInfos = rootCommandInfos
            .Select((infos, ctx) => builder.BindParentsAndRemoveDuplicates(infos))
            .WithTrackingName(TrackingNames.BindParentsAndRemoveDuplicates);

        var allCommandInfos = boundRootCommandInfos
            .SelectMany(static (collected, _) => collected.SelectMany(commandInfo => builder.GetSelfAndDescendants(commandInfo)))
            .WithTrackingName(TrackingNames.FlattenHierarchy);

        var collectedAllCommandInfos = allCommandInfos.Collect();

        context.RegisterSourceOutput(collectedAllCommandInfos,
                static (spc, cmdInfos) =>
                {
                    builder.OutputCliSource(spc, cmdInfos);
                });

        context.RegisterSourceOutput(allCommandInfos,
            static (spc, collected) =>
        {
            builder.OutputSource(spc, collected);
        });
    }

}
