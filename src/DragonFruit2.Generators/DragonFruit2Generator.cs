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
                predicate: DragonFruit2Builder.InitialEntryPointFilter,
                transform: DragonFruit2Builder.TransformEntryPoint)
            .WithTrackingName("ParseArgsInvocations")
            .Where(static s => s is not null)
            .Select(static (s, _) => s!) // Quiet nullability warning
            .WithTrackingName(TrackingNames.ExtractEntryPoint);

        var cliInfosByNamespace = cliInfos
            .Collect()
            .Select((infos, ctx) => CommandBuilder.GetCliInfoGroups(infos, ctx))
            .SelectMany((x, ctx) => x)
            .WithTrackingName(TrackingNames.BuildCliInfoGroups);

        context.RegisterSourceOutput(cliInfosByNamespace,
                                     DragonFruit2Builder.OutputCliSource);

        var commandInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: "DragonFruit2.CommandClassAttribute",
                predicate: DragonFruit2Builder.FilterForClassDeclarations,
                transform: DragonFruit2Builder.TransformCommandClasses)
            .WithTrackingName("ParseArgsInvocations")
            .Where(static s => s is not null)
            .Select(static (s, _) => s!) // Quiet nullability warning
            .WithTrackingName(TrackingNames.ExtractCommandClasses);

        var commandNodes = commandInfos
            .Collect()
            .Select((infos, ctx) => CommandBuilder.BuildCommandNodes(infos, ctx))
            .SelectMany((x, ctx) => x)
            .WithTrackingName(TrackingNames.BuildCommandNodes);

        context.RegisterSourceOutput(commandNodes,
            DragonFruit2Builder.OutputCommandPartialSource);
    }

}
