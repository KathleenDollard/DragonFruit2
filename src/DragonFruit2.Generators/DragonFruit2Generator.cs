using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators;

[Generator]
public sealed partial class DragonFruit2Generator : IIncrementalGenerator
{
    private static readonly DragonFruit2Builder builder = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Filter the ParseAxgs invocation syntax and transorm to CliInfo
        var cliInfos = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: DragonFruit2Builder.InitialEntryPointFilter,
                transform: DragonFruit2Builder.TransformEntryPoint)
            .WithTrackingName("ParseArgsInvocations")
            .Where(static s => s is not null)
            .Select(static (s, _) => s!) // Quiet nullability warning
            .WithTrackingName(TrackingNames.ExtractEntryPoint);

        // Create CliInfoGroup for each namespace
        // This allows one file to be generated for each namespace
        var cliInfosByNamespace = cliInfos
            .Collect()
            .Select((infos, ctx) => CommandBuilder.GetCliInfoGroups(infos, ctx))
            .SelectMany((x, ctx) => x)
            .WithTrackingName(TrackingNames.BuildCliInfoGroups);

        context.RegisterSourceOutput(cliInfosByNamespace,
                                     DragonFruit2Builder.OutputCliSource);

        // Find all CommandClasses and transform to CommandInfo instances
        var commandInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: "DragonFruit2.CommandClassAttribute",
                predicate: DragonFruit2Builder.FilterForClassDeclarations,
                transform: DragonFruit2Builder.TransformCommandClasses)
            .Where(static s => s is not null)
            .Select(static (s, _) => s!) // Quiet nullability warning
            .WithTrackingName(TrackingNames.ExtractCommandClasses);

        // Create a tree of nodes; each node includes its CommandInfo 
        // and nodes for its subcommands; allowing one file
        // per CommandInfo/CommandInfo
        var commandNodes = commandInfos
            .Collect()
            .Select((infos, ctx) => CommandBuilder.BuildCommandNodes(infos, ctx))
            .SelectMany((x, ctx) => x)
            .WithTrackingName(TrackingNames.BuildCommandNodes);

        context.RegisterSourceOutput(commandNodes,
            DragonFruit2Builder.OutputCommandPartialSource);
    }

}
