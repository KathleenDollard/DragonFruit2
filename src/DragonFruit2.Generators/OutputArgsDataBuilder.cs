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
                $"internal class {commandInfo.Name}ArgsBuilder : ArgsBuilder<{commandInfo.Name}>"]);
        sb.OpenCurly();
    }

    private static void Initialize(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        var commandDescription = commandInfo.Description is null
                                  ? "null"
                                  : $"\"{commandInfo.Description.Replace("\"", "\"\"")}\"";
        sb.OpenMethod($"""public override void Initialize(Builder<{commandInfo.Name}> builder)""");

        sb.AppendLine($"""var cliDataProvider = GetCliDataProvider(builder);""");

        sb.AppendLine($"""var rootCommand = new System.CommandLine.Command("{commandInfo.Name}")""");
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
        sb.AppendLine($"""cliDataProvider.RootCommand = rootCommand;""");

        sb.CloseMethod();
    }

    internal static void GetOptionDeclaration(StringBuilderWrapper sb, PropInfo propInfo)
    {
        var description = propInfo.Description is null
                              ? "null"
                              : $"\"{propInfo.Description.Replace("\"", "\"\"")}\"";
        string symbolName = $"{OutputHelpers.GetLocalSymbolName(propInfo.Name)}Option";
        sb.AppendLine($"""var {symbolName} = new Option<{propInfo.TypeName}>("--{propInfo.CliName}")""");
        sb.OpenCurly();
        sb.AppendLines([
                $"Description = {description},",
                $"""Required = {(propInfo.IsRequiredForCli ? "true" : "false")}"""]);
        sb.CloseCurly(endStatement: true);

        AddSymbolToLookup(sb, propInfo, symbolName);
        AddSymbolToRootCommand(sb, symbolName);
    }

    internal static void GetArgumentDeclaration(StringBuilderWrapper sb, PropInfo propInfo)
    {
        var description = propInfo.Description is null
                              ? "null"
                              : $"\"{propInfo.Description.Replace("\"", "\"\"")}\"";
        string symbolName = $"{OutputHelpers.GetLocalSymbolName(propInfo.Name)}Argument";
        sb.AppendLine($"""var {symbolName} = new Argument<{propInfo.TypeName}>("{propInfo.Name}")""");
        sb.OpenCurly();
        sb.AppendLine($"""
                Description = {description},
                Required = {(propInfo.IsRequiredForCli ? "true" : "false")}
                """);
        sb.CloseCurly(true, endStatement: true);
        AddSymbolToLookup(sb, propInfo, symbolName);
        AddSymbolToRootCommand(sb, symbolName);
    }

    internal static void GetSubCommandDeclaration(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        string symbolName = $"{OutputHelpers.GetLocalSymbolName(commandInfo.Name)}Command";
        sb.AppendLine($"""{symbolName}Command = new System.CommandLine.Command("Root")""");
        AddSymbolToRootCommand(sb, symbolName);
    }

    private static void AddSymbolToLookup(StringBuilderWrapper sb, PropInfo propInfo, string symbolName)
    {
        sb.AppendLine($"""cliDataProvider.AddNameLookup("{propInfo.Name}", {symbolName});""");
    }

    private static void AddSymbolToRootCommand(StringBuilderWrapper sb, string symbolName)
    {
        sb.AppendLine($"""rootCommand.Add({symbolName});""");
    }

    private static void CreateInstance(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""protected override {commandInfo.Name} CreateInstance(Builder<{commandInfo.Name}> builder)""");

        foreach (var propInfo in commandInfo.PropInfos)
        {
            sb.AppendLine($"""var {propInfo.Name.ToCamelCase()}DataValue = builder.GetDataValue<{propInfo.TypeName}>("{propInfo.Name}");""");
        }
        var ctorArguments = commandInfo.PropInfos.Select(p => $"{p.Name.ToCamelCase()}DataValue");
        sb.AppendLine();
        sb.Append($"""var newArgs = new {commandInfo.Name}({string.Join(", ", ctorArguments)});""");
        sb.AppendLine();
        sb.AppendLine($"""return newArgs;""");
        sb.CloseMethod();

    }

    private static void CheckRequiredValues(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""protected override IEnumerable<ValidationFailure> CheckRequiredValues(Builder<{commandInfo.Name}> builder)""");

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

