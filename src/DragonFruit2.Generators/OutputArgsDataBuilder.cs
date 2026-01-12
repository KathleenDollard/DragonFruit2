using DragonFruit2.GeneratorSupport;
using System.Text;

namespace DragonFruit2.Generators;

internal static class OutputArgsBuilder
{
    internal static void GetClass(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        OpenClass(commandInfo, sb);

        Initialize(sb, commandInfo);
        //OutputInitializeMethod(commandInfo, sb); // moved to builder
        sb.AppendLine();
        CheckRequiredValues(sb, commandInfo);  // not yet implemented
        //OutputStaticCreateMethods(commandInfo, sb); // moved to builder
        CreateInstance(sb, commandInfo);

        sb.CloseClass();
    }
    internal static void OpenClass(CommandInfo commandInfo, StringBuilderWrapper sb)
    {
        sb.AppendLine();
        sb.AppendLines([
                "/// <summary>",
                "/// </summary>",
                $"internal class {commandInfo.Name}ArgsBuilder : ArgsBuilder<{commandInfo.RootName}>"]);
        sb.OpenCurly();
        //sb.AppendLine($$"""public ArgsBuilder<{{commandInfo.RootName}}>? ActiveArgsBuilder { get; set; }""");
    }

    private static void Initialize(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        var commandDescription = commandInfo.Description is null
                                  ? "null"
                                  : $"\"{commandInfo.Description.Replace("\"", "\"\"")}\"";
        sb.OpenMethod($"""public override void Initialize(Builder<{commandInfo.RootName}> builder, bool isRoot=false)""");

        sb.AppendLine($"""var cliDataProvider = GetCliDataProvider(builder);""");

        sb.AppendLine($"""var cmd = new System.CommandLine.Command("{commandInfo.Name}")""");
        sb.OpenCurly();
        sb.AppendLine($"""Description = {commandDescription},""");
        sb.CloseCurly(endStatement: true);

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

        sb.AppendLine("cmd.SetAction(p => { ActiveArgsBuilder = this; return 0; });");
        sb.OpenIf("isRoot");
        sb.AppendLine($"""cliDataProvider.RootCommand = cmd;""");
        sb.CloseIf();

        sb.CloseMethod();
    }

    internal static void GetOptionDeclaration(StringBuilderWrapper sb, PropInfo propInfo)
    {
        var description = propInfo.Description is null
                              ? "null"
                              : $"\"{propInfo.Description.Replace("\"", "\"\"")}\"";
        string symbolName = $"{OutputHelpers.GetLocalSymbolName(propInfo.Name)}Option";
        sb.AppendLine($"""var {symbolName} = new Option<{propInfo.TypeName}>("{propInfo.CliName}")""");
        sb.OpenCurly();
        sb.AppendLines([
                $"Description = {description},",
                $"""Required = {(propInfo.IsRequiredForCli ? "true" : "false")},""",
                "Recursive=true"]);
        sb.CloseCurly(endStatement: true);

        AddSymbolToLookup(sb, propInfo, symbolName);
        sb.AppendLine($"""cmd.Add({symbolName});""");
    }

    internal static void GetArgumentDeclaration(StringBuilderWrapper sb, PropInfo propInfo)
    {
        var description = propInfo.Description is null
                              ? "null"
                              : $"\"{propInfo.Description.Replace("\"", "\"\"")}\"";
        string symbolName = $"{OutputHelpers.GetLocalSymbolName(propInfo.Name)}Argument";
        sb.AppendLine($"""var {symbolName} = new Argument<{propInfo.TypeName}>("{propInfo.CliName}")""");
        sb.OpenCurly();
        sb.AppendLine($"""
                Description = {description},
                Required = {(propInfo.IsRequiredForCli ? "true" : "false")}
                """);
        sb.CloseCurly(true, endStatement: true);
        AddSymbolToLookup(sb, propInfo, symbolName);
        sb.AppendLine($"""cmd.Add({symbolName});""");
    }

    internal static void GetSubCommandDeclaration(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        string symbolName = $"{OutputHelpers.GetLocalSymbolName(commandInfo.Name)}Command";
        sb.AppendLine($"""var {symbolName} = new System.CommandLine.Command("{commandInfo.CliName ?? "Root"}");""");
        sb.AppendLine($"{commandInfo.Name}.GetArgsBuilder(builder).Initialize(builder);");

        sb.AppendLine($"""cmd.Add({symbolName});""");
    }

    private static void AddSymbolToLookup(StringBuilderWrapper sb, PropInfo propInfo, string symbolName)
    {
        sb.AppendLine($"""cliDataProvider.AddNameLookup("{propInfo.Name}", {symbolName});""");
    }

    private static void CreateInstance(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""protected override {commandInfo.RootName} CreateInstance(Builder<{commandInfo.RootName}> builder)""");

        foreach (var propInfo in commandInfo.SelfAndAncestorPropInfos)
        {
            sb.AppendLine($"""var {propInfo.Name.ToCamelCase()}DataValue = builder.GetDataValue<{propInfo.TypeName}>("{propInfo.Name}");""");
        }
        var ctorArguments = commandInfo.SelfAndAncestorPropInfos.Select(p => $"{p.Name.ToCamelCase()}DataValue");
        sb.AppendLine();
        sb.Append($"""var newArgs = new {commandInfo.Name}({string.Join(", ", ctorArguments)});""");
        sb.AppendLine();
        sb.AppendLine($"""return newArgs;""");
        sb.CloseMethod();

    }

    private static void CheckRequiredValues(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""protected override IEnumerable<ValidationFailure> CheckRequiredValues(Builder<{commandInfo.RootName}> builder)""");

        var requiredValues = commandInfo.PropInfos.Where(p => p.IsRequiredForCli).ToList();
        sb.AppendLine($"var validationFailures = new List<ValidationFailure?>();");

        foreach (var requiredValue in requiredValues)
        {
            sb.AppendLine($"""validationFailures.Add(CheckRequiredValue<{requiredValue.TypeName}>("{requiredValue.Name}", builder.GetDataValue<string>("{requiredValue.Name}")));""");
        }

        sb.AppendLine($"""
            return validationFailures
                       .Where(x => x is not null)
                       .Select(x => x!);
            """);

        sb.CloseMethod();
    }

}

