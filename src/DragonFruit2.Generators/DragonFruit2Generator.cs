using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators;

[Generator]
public sealed partial class DragonFruit2Generator : IIncrementalGenerator
{
    private static readonly DragonFruit2Builder builder = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var cliInfos = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: builder.InitialEntryPointFilter,
                transform: static (context, cancelToken) => builder.TransformEntryPoint(context, cancelToken))
            .WithTrackingName("ParseArgsInvocations")
            .Where(static s => s is not null)
            .Select(static (s, _) => s!) // Quiet nullability warning
            .Collect()
            .WithTrackingName(TrackingNames.Extract);

        var commandInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: "DragonFruit2.CommandClass",
                predicate: static (node, _) => true,
                transform: static (ctx, _) => builder.TransformCommandClasses(ctx))
            .WithTrackingName("ParseArgsInvocations")
            .Where(static s => s is not null)
            .Select(static (s, _) => s!) // Quiet nullability warning
            .Collect()
            .WithTrackingName(TrackingNames.Extract);

        var commandRootsWithTrees = commandInfos
            .Select((infos, ctx) => CommandBuilder.BuildCommandTree(infos))
            .WithTrackingName(TrackingNames.BuildTrees);

        var cliInfosWithTrees = cliInfos
            .Combine(commandRootsWithTrees)
            .Select(CommandBuilder.CreateHierarchyAndGroup)
            .WithTrackingName(TrackingNames.BuildTrees);

        var spreadCliInfoGroups = cliInfosWithTrees
            .SelectMany(CommandBuilder.SpreadCliInfos);

        // TODO: Group by namespace and SelectMany for separate files for each

        // This allows generating a file per command 
        var individualCommands = commandRootsWithTrees
            .SelectMany((commandNodes, ctx) => CommandBuilder.FlattenHierarchy(commandNodes, ctx))
            .WithTrackingName(TrackingNames.FlattenHierarchy);

        context.RegisterSourceOutput(spreadCliInfoGroups,
                static (spc, groupedCli) =>
                {
                    builder.OutputCliSource(spc, groupedCli);
                });

        context.RegisterSourceOutput(individualCommands,
            static (spc, commandClass) =>
        {
            builder.OutputCommandPartialSource(spc, commandClass);
        });
    }

}
