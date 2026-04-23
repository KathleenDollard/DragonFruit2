using DragonFruit2.Generators.CodeOutput;
using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DragonFruit2.Generators;

public class DragonFruit2Builder
{
    private enum CliSymbolType
    {
        Option,
        Argument,
        SubCommand
    }

    /// <summary>
    /// Determines whether the specified syntax node represents an invocation of the generic method 'ParseArgs' with
    /// exactly one type argument.
    /// </summary>
    /// <param name="node">The syntax node to evaluate.</param>
    /// <returns>true if the node is an invocation of 'ParseArgs' with a single type argument; otherwise, false.</returns>
    public bool InitialEntryPointFilter(SyntaxNode node, CancellationToken _)
    {
        if (node is null) return false;

        try
        {
            return (node is InvocationExpressionSyntax inv) &&
                inv.Expression switch
                {
                    MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                        => IsMethodNameOfInterest(gns.Identifier.ValueText) && gns.TypeArgumentList.Arguments.Count == 1,
                    GenericNameSyntax gns
                        => IsMethodNameOfInterest(gns.Identifier.ValueText) && gns.TypeArgumentList.Arguments.Count == 1,
                    _ => false,
                };
        }
        catch
        {
            throw;
        }

        static bool IsMethodNameOfInterest(string valueText)
        {
            return valueText == "ParseArgs" || valueText == "TryParseArgs" || valueText == "TryExecute";
        }
    }

    public CliInfo? TransformEntryPoint(GeneratorSyntaxContext context, CancellationToken _)
    {
        try
        {
            // We only get here for ParseArg invocations with a single generic type argument
            return context.Node switch
            {
                InvocationExpressionSyntax invocationSyntax
                       => CliBuilder.GetCliInfo(invocationSyntax, context.SemanticModel),
                _ => null,
            };
        }
        catch
        {
            throw;
        }
    }

    public CommandInfo? TransformCommandClasses(GeneratorAttributeSyntaxContext context)
    {
        try
        {
            // We only get here for ParseArg invocations with a single generic type argument
            return context.TargetNode switch
            {
                ClassDeclarationSyntax classDeclarationSyntax
                       => CommandBuilder.GetCommandInfo(classDeclarationSyntax, context.SemanticModel),
                _ => null,
            };
        }
        catch
        {
            throw;
        }
    }

    public void OutputCommandPartialSource(SourceProductionContext context, CommandNode commandNode)
    {
        try
        {
            context.AddSource(commandNode.CommandInfo.Name, OutputPartialArgs.GetSourcePartialArgs(commandNode));
        }
        catch
        {
            throw;
        }
    }

    public void OutputCliSource(SourceProductionContext context, CliInfoGroup cliInfoGroup)
    {
        try
        {
            
                context.AddSource("Cli", OutputCli.GetSource(cliInfoGroup.EntryPointNamespace, cliInfoGroup.CliInfos));
        }
        catch
        {
            throw;
        }
    }
}
