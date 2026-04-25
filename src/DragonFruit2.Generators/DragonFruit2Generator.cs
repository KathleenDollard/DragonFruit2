using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators;

[Generator]
public sealed partial class DragonFruit2Generator : IIncrementalGenerator
{
    private static readonly DragonFruit2Builder builder = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Lambda's are used to allow IEnumerable parameters for testing. If this is identified 
        // as a perf issue, an extra call can be made, or tests can create ImmutableArrays.
        // (If the signature matched exactly, method groups can be used instead of lambdas)

        var cliInfos = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: builder.InitialEntryPointFilter,
                transform: static (context, cancelToken) => builder.TransformEntryPoint(context, cancelToken))
            .WithTrackingName("ParseArgsInvocations")
            .Where(static s => s is not null)
            .Select(static (s, _) => s!) // Quiet nullability warning
            .Collect()
            .WithTrackingName(TrackingNames.Extract);

        var cliInfosGroups = cliInfos
            .Select((infos, ctx) => CommandBuilder.GetCliInfoGroups(infos, ctx))
            .WithTrackingName(TrackingNames.BuildCliInfoGroups);

        var spreadCliInfoGroups = cliInfosGroups
            .SelectMany((x, ctx) => x);

        context.RegisterSourceOutput(spreadCliInfoGroups,
                static (spc, groupedCli) =>
                {
                    builder.OutputCliSource(spc, groupedCli);
                });


        var commandInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: "DragonFruit2.CommandClassAttribute",
                predicate: static (node, _) => true,
                transform: static (ctx, _) => builder.TransformCommandClasses(ctx))
            .WithTrackingName("ParseArgsInvocations")
            .Where(static s => s is not null)
            .Select(static (s, _) => s!) // Quiet nullability warning
            .Collect()
            .WithTrackingName(TrackingNames.Extract);

        var commandRootsWithTrees = commandInfos
            .Select((infos, ctx) => CommandBuilder.BuildCommandTree(infos, ctx))
            .WithTrackingName(TrackingNames.BuildTrees);

        // This allows generating a file per command 
        var individualCommands = commandRootsWithTrees
            .SelectMany((commandNodes, ctx) => CommandBuilder.FlattenHierarchy(commandNodes, ctx))
            .WithTrackingName(TrackingNames.FlattenHierarchy);

        context.RegisterSourceOutput(individualCommands,
            static (spc, commandClass) =>
        {
            builder.OutputCommandPartialSource(spc, commandClass);
        });
    }

}
