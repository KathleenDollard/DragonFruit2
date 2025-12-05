using DragonFruit2.GeneratorSupport;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace DragonFruit2.Generators;

public class DragonFruit2Builder : GeneratorBuilder<CommandInfo>
{
    private static readonly int indentSize = 4;
    private static string indent = "";

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
    public override bool InitialFilter(SyntaxNode node)
        => (node is InvocationExpressionSyntax inv) &&
        inv.Expression switch
        {
            MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                => gns.Identifier.ValueText == "ParseArgs" && gns.TypeArgumentList.Arguments.Count == 1,
            GenericNameSyntax gns2
                => gns2.Identifier.ValueText == "ParseArgs" && gns2.TypeArgumentList.Arguments.Count == 1,
            _ => false,
        };

    public override CommandInfo? Transform(GeneratorSyntaxContext context)
            // We only get here for ParseArg invocations with a single generic type argument
            => context.Node switch
            {
                InvocationExpressionSyntax invocationSyntax
                       => GetRootCommandInfoFromInvocation(invocationSyntax, context.SemanticModel),
                _ => null,
            };

    public override void OutputSource(SourceProductionContext context, IEnumerable<CommandInfo> items)
    {
        foreach (var commandInfo in items)
        {
            var sourceText = GetSourceForCommandInfo(commandInfo);
            context.AddSource($"{commandInfo.Name}_ParseArgsGenerator.g.cs", sourceText);
        }
    }


    public static CommandInfo? GetRootCommandInfoFromInvocation(InvocationExpressionSyntax invocationSyntax, SemanticModel semanticModel)
    {
        var rootArgsTypeArgSymbol = GetArgsTypeSymbol(invocationSyntax, semanticModel);
        if (rootArgsTypeArgSymbol is null)
            return null; // This occurs when the root arg type does not yet exist
        var rootCommandInfo = CreateCommandInfo(rootArgsTypeArgSymbol, semanticModel);
        return rootCommandInfo;
    }

    private static CommandInfo CreateCommandInfo(INamedTypeSymbol typeSymbol, SemanticModel semanticModel)
    {
        var commandInfo = CommandInfoHelpers.CreateCommandInfo(typeSymbol);

        var props = typeSymbol.GetMembers()
                              .OfType<IPropertySymbol>()
                              .Where(p => !p.IsStatic)
                              .Select(p => CommandInfoHelpers.CreatePropInfo(p))
                              .ToList();

        // Split into argument list and options
        var argList = props.Where(p => p.IsArgument).OrderBy(p => p.Position).ToList();
        var optList = props.Where(p => !p.IsArgument).ToList();
        commandInfo.Arguments.AddRange(argList);
        commandInfo.Options.AddRange(optList);

        var compilation = semanticModel.Compilation;

        var derivedTypes = GetChildTypes(typeSymbol);
        foreach (var derivedType in derivedTypes)
        {
            var childCommandInfo = CreateCommandInfo(derivedType, semanticModel);
            commandInfo.SubCommands.Add(childCommandInfo);
        }

        return commandInfo;
    }

    private static IEnumerable<INamedTypeSymbol> GetChildTypes(INamedTypeSymbol typeSymbol)
    {
        var derivedTypes = new List<INamedTypeSymbol>();
        var nspace = typeSymbol.ContainingNamespace;
        foreach (var member in nspace.GetMembers())
        {
            if (member is INamedTypeSymbol namedTypeSymbol)
            {
                // Check if this type derives from the given typeSymbol
                if (SymbolEqualityComparer.Default.Equals(namedTypeSymbol.BaseType, typeSymbol))
                {
                    // This namedTypeSymbol derives from typeSymbol
                    derivedTypes.Add(namedTypeSymbol.BaseType);
                    break;
                }
            }
        }
        return derivedTypes;
    }

    private static INamedTypeSymbol? GetArgsTypeSymbol(InvocationExpressionSyntax? invocation, SemanticModel semanticModel)
    {
        if (invocation is null) return null;

        GenericNameSyntax? genericName = invocation.Expression switch
        {
            MemberAccessExpressionSyntax ma when ma.Name is GenericNameSyntax gns
                => gns,
            GenericNameSyntax gns
                => gns,
            _ => null,
        };

        if (genericName is null) return null;

        var typeArgSyntax = genericName.TypeArgumentList.Arguments[0];

        return semanticModel.GetSymbolInfo(typeArgSyntax).Symbol as INamedTypeSymbol;
    }

    internal static string GetSourceForCommandInfo(CommandInfo commandInfo)
    {
        var sb = new StringBuilder();
        sb.Append(OutputFileOpening());
        OpenNamespace(commandInfo, sb);
        OpenArgsPartialClass(commandInfo, sb);

        OutputInitializeMethod(commandInfo, sb);
        sb.AppendLine();
        OutputConstructors(commandInfo, sb);
        OutputStaticCreateMethods(commandInfo, sb);

        CloseCurly(sb);
        CloseNamespace(commandInfo, sb);
        return sb.ToString();
    }

    private static void OutputStaticCreateMethods(CommandInfo commandInfo, StringBuilder sb)
    {
        sb.AppendLine($"""{indent}public static {commandInfo.Name} Create(Builder<{commandInfo.Name}> builder)""");
        OpenCurly(sb);
        foreach (var propInfo in commandInfo.PropInfos)
        {
            sb.AppendLine($"""{indent}var {propInfo.Name.ToCamelCase()}DataValue = GetDataValue<{propInfo.TypeName}>("{propInfo.Name}")""");
        }
        sb.AppendLine();
        sb.Append($"""{indent}var newArgs = new {commandInfo.Name}(""");
        foreach (var propInfo in commandInfo.PropInfos)
        {
            sb.Append($"""{propInfo.Name.ToCamelCase()}DataValue, """);
        }
        sb.AppendLine(")");
        sb.AppendLine($"""{indent}return newArgs;""");
        CloseCurly(sb);

    }

    private static void OutputConstructors(CommandInfo commandInfo, StringBuilder sb)
    {
        // TODO: Only generate the following constructor if the user does not create it
        sb.AppendLine($"""{indent}public {commandInfo.Name}()""");
        OpenCurly(sb);
        CloseCurly(sb);

        sb.AppendLine();
        sb.AppendLine($"""{indent}[SetsRequiredMembers()]""");
        sb.Append($"""{indent}private {commandInfo.Name}(""");

        foreach (var propInfo in commandInfo.PropInfos)
        {
            sb.Append($"""DataValue<{propInfo.TypeName}> {propInfo.Name.ToCamelCase()}DataValue, """);
        }
        sb.AppendLine(")");
        sb.AppendLine($"""{indent}    : this()""");
        OpenCurly(sb);
        foreach (var propInfo in commandInfo.PropInfos)
        {
            sb.AppendLine($"""{indent}if ({propInfo.Name.ToCamelCase()}DataVaue.IsSet) {propInfo.Name}DataVaue = {propInfo.Name.ToCamelCase()}DataVaue.Value;""");
        }
        CloseCurly(sb);
        sb.AppendLine();
    }

    private static void OutputInitializeMethod(CommandInfo commandInfo, StringBuilder sb)
    {
        var commandDescription = commandInfo.Description is null
                                  ? "null"
                                  : $"\"{commandInfo.Description.Replace("\"", "\"\"")}\"";
        sb.AppendLine($"""{indent}public static void Initialize(Builder<{commandInfo.Name}> builder)""");
        OpenCurly(sb);
        sb.AppendLine($"""{indent}var cliDataProvider = builder.DataProviders.OfType<CliDataProvider<{commandInfo.Name}>>().FirstOrDefault();""");
        sb.AppendLine($"""{indent}if (cliDataProvider is null)""");
        OpenCurly(sb);
        sb.AppendLine($"""{indent}cliDataProvider = new CliDataProvider<{commandInfo.Name}>();""");
        sb.AppendLine($"""{indent}builder.DataProviders.Add(cliDataProvider);""");
        CloseCurly(sb);
        sb.AppendLine();
        sb.AppendLine($"""{indent}var rootCommand = new System.CommandLine.Command("{commandInfo.Name}")""");
        OpenCurly(sb);
        sb.AppendLine($"""{indent}Description = {commandDescription},""");
        CloseCurly(sb, endStatement: true);

        sb.AppendLine();
        foreach (var option in commandInfo.Options)
        {
            GetOptionDeclaration(sb, option);
        }
        foreach (var argument in commandInfo.Arguments)
        {
            GetArgumentDeclaration(sb, argument);
        }
        foreach (var subcommand in commandInfo.SubCommands)
        {
            GetSubCommandDeclaration(sb, subcommand);
        }
        sb.AppendLine($"""{indent}cliDataProvider.RootCommand = rootCommand;""");

        CloseCurly(sb);
    }

    private static void OpenArgsPartialClass(CommandInfo commandInfo, StringBuilder sb)
    {
        var classOrStruct = commandInfo.IsStruct ? "struct" : "class";
        sb.AppendLine($"""
                {indent}/// <summary>
                {indent}/// Auto-generated partial {classOrStruct} for building CLI commands for <see cref="{commandInfo.Name}" />
                {indent}/// and creating a new {commandInfo.Name} instance from a <see cref="System.CommandLine.ParseResult" />.
                {indent}/// </summary>
                {indent}public partial {classOrStruct} {commandInfo.Name} : IArgs<{commandInfo.Name}>
                """);
        OpenCurly(sb);
    }

    private static void OpenNamespace(CommandInfo commandInfo, StringBuilder sb)
    {
        sb.AppendLine();
        if (!string.IsNullOrEmpty(commandInfo.NamespaceName))
        {
            sb.AppendLine($"{indent}namespace {commandInfo.NamespaceName}");
            OpenCurly(sb);
        }
    }

    private static void CloseNamespace(CommandInfo commandInfo, StringBuilder sb)
    {
        if (!string.IsNullOrEmpty(commandInfo.NamespaceName))
        {
            CloseCurly(sb);
        }
    }

    private static string GetLocalSymbolName(string name)
    {
        return $"{char.ToLower(name[0])}{name.Substring(1)}";
    }

    private static void OpenCurly(StringBuilder sb)
    {
        sb.AppendLine($$"""{{indent}}{""");
        indent += new string(' ', indentSize);
    }

    private static void CloseCurly(StringBuilder sb, bool closeParens = false, bool endStatement = false)
    {
        indent = indent.Substring(indentSize);
        sb.AppendLine($$"""{{indent}}}{{(closeParens ? ")" : "")}}{{(endStatement ? ";" : "")}}""");
    }

    private static string OutputFileOpening()
    {
        return $"""
                       // <auto-generated />
                       using DragonFruit2;
                       using System.CommandLine;
                       using System.Diagnostics.CodeAnalysis;

                       """;
    }

    internal static void GetSubCommandDeclaration(StringBuilder sb, CommandInfo commandInfo)
    {
        string symbolName = $"{GetLocalSymbolName(commandInfo.Name)}Command";
        sb.AppendLine($"""{indent}{symbolName}Command = new System.CommandLine.Command("Test")""");
        AddSymbolToRootCommand(sb, symbolName);
    }

    internal static void GetArgumentDeclaration(StringBuilder sb, PropInfo propInfo)
    {
        var description = propInfo.Description is null
                              ? "null"
                              : $"\"{propInfo.Description.Replace("\"", "\"\"")}\"";
        string symbolName = $"{GetLocalSymbolName(propInfo.Name)}Argument";
        sb.AppendLine($"""{indent}var {symbolName} = new Argument<{propInfo.TypeName}>("{propInfo.Name}")""");
        OpenCurly(sb);
        sb.AppendLine($"""
                {indent}Description = {description},
                {indent}Required = {(propInfo.IsRequiredForCli ? "true" : "false")}
                """);
        CloseCurly(sb, true, endStatement: true);
        AddSymbolToLookup(sb, propInfo, symbolName);
        AddSymbolToRootCommand(sb, symbolName);
    }

    internal static void GetOptionDeclaration(StringBuilder sb, PropInfo propInfo)
    {
        var description = propInfo.Description is null
                              ? "null"
                              : $"\"{propInfo.Description.Replace("\"", "\"\"")}\"";
        string symbolName = $"{GetLocalSymbolName(propInfo.Name)}Option";
        sb.AppendLine($"""{indent}var {symbolName} = new Option<{propInfo.TypeName}>("--{propInfo.CliName}")""");
        OpenCurly(sb);
        sb.AppendLine($"""
                {indent}Description = {description},
                {indent}Required = {(propInfo.IsRequiredForCli ? "true" : "false")}
                """);
        CloseCurly(sb, endStatement: true);
        AddSymbolToLookup(sb, propInfo, symbolName);
        AddSymbolToRootCommand(sb, symbolName);
    }

    private static void AddSymbolToLookup(StringBuilder sb, PropInfo propInfo, string symbolName)
    {
        sb.AppendLine($"""{indent}builder.AddNameLookup("{propInfo.Name}", {symbolName});""");
    }

    private static void AddSymbolToRootCommand(StringBuilder sb, string symbolName)
    {
        sb.AppendLine($"""{indent}rootCommand.Add({symbolName});""");
    }


}
