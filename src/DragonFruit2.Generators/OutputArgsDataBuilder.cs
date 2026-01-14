using DragonFruit2.GeneratorSupport;
using System.Text;

namespace DragonFruit2.Generators;

internal static class OutputArgsBuilder
{
    internal static void GetClass(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        OpenClass(commandInfo, sb);

        Initialize(sb, commandInfo);
        sb.AppendLine();
        CheckRequiredValues(sb, commandInfo);  // not yet implemented
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

    internal static void StaticConstructor(CommandInfo commandInfo, StringBuilderWrapper sb)
    {
        sb.XmlSummary("This static builder supplies the CLI declaration and filling the Result and return instance.");
        sb.XmlRemarks("The first type argument of the base is the Args type this builder creates, and the second is the root Args type. This means the two type arguments are the same for the root ArgsBuilder, but will differ for subcommand ArgsBuilders.");
        sb.OpenMethod($"""static {commandInfo.Name}ArgsBuilder()""");
        sb.AppendLine($"""ArgsBuilderCache<{commandInfo.RootName}>.AddArgsBuilder<{commandInfo.Name}> (new {commandInfo.Name}ArgsBuilder());""");
        sb.CloseMethod();

    }

    private static void Initialize(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""public override void Initialize(Builder<{commandInfo.RootName}> builder)""");
        sb.AppendLine($"InitializeCli(builder, builder.DataProviders.OfType<CliDataProvider<{commandInfo.RootName}>>().FirstOrDefault());");
        sb.CloseMethod();

        InitializeCli(sb, commandInfo);
    }

    private static void InitializeCli(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        var commandDescription = commandInfo.Description is null
                                  ? "null"
                                  : $"\"{commandInfo.Description.Replace("\"", "\"\"")}\"";
        sb.OpenMethod($"""public override Command InitializeCli(Builder<{commandInfo.RootName}> builder, CliDataProvider<{commandInfo.RootName}>? cliDataProvider)""");

        sb.AppendLine($"""var cmd = new System.CommandLine.{(commandInfo.BaseName is null ? "Root" : "")}Command("{commandInfo.CliName}")""");
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

        sb.AppendLine($$"""cmd.SetAction(p => { ArgsBuilderCache<{{commandInfo.RootName}}>.ActiveArgsBuilder = this; return {{commandInfo.Name.Length + commandInfo.BaseName?.Length}}; });""");

        if (commandInfo.Name == commandInfo.RootName)
        {
            sb.AppendLine("cliDataProvider.RootCommand = cmd;");
        }

        sb.AppendLine("return cmd;");

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
        sb.AppendLine();

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
        sb.AppendLine();
    }

    internal static void GetSubCommandDeclaration(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        string symbolName = $"{OutputHelpers.GetLocalSymbolName(commandInfo.Name)}Command";
        sb.AppendLine($"""var {symbolName} = {commandInfo.Name}.GetArgsBuilder(builder).InitializeCli(builder, cliDataProvider);""");
        sb.AppendLine($"""cmd.Add({symbolName});""");
        sb.AppendLine();
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

