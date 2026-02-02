namespace DragonFruit2.Generators;

internal static class OutputArgsBuilder
{
    internal static void GetClass(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        OpenClass(commandInfo, sb);

        Initialize(sb, commandInfo);
        sb.AppendLine();
        CheckRequiredValues(sb, commandInfo);  // not yet implemented
        CreateDataValues(sb, commandInfo);
        CreateInstance(sb, commandInfo);

        sb.CloseClass();
    }

    internal static void OpenClass(CommandInfo commandInfo, StringBuilderWrapper sb)
    {
        sb.AppendLine();
        sb.XmlSummary(" This static builder supplies the CLI declaration and filling the Result and return instance.");
        sb.OpenClass($"internal class {commandInfo.Name}ArgsBuilder : ArgsBuilder<{commandInfo.RootName}>");
    }


    private static void Initialize(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""public override void Initialize(Builder<{commandInfo.RootName}> builder)""");
        sb.AppendLine($"InitializeCli(builder, builder.GetDataProvider<CliDataProvider<{commandInfo.RootName}>>());");
        sb.AppendLine($"InitializeDefaults(builder, builder.GetDataProvider<DefaultDataProvider<{commandInfo.RootName}>>());");
        sb.CloseMethod();

        InitializeCli(sb, commandInfo);
        InitializeDefaults(sb, commandInfo);
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
        foreach (var optionInfo in commandInfo.Options)
        {
            GetOptionDeclaration(sb, commandInfo, optionInfo);
        }
        foreach (var argumentInfo in commandInfo.Arguments)
        {
            GetArgumentDeclaration(sb, commandInfo,argumentInfo);
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

    private static void InitializeDefaults(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""public void InitializeDefaults(Builder<{commandInfo.RootName}> builder, DefaultDataProvider<{commandInfo.RootName}>? defaultDataProvider)""");
        sb.AppendLine("if (defaultDataProvider is null) return;");
        sb.Comment("TODO: Register defaults based on attributes, initializer, and the RegisterDefault calls");
        sb.AppendLine("RegisterCustomDefaults(builder, defaultDataProvider);");
        sb.CloseMethod();
    }

    internal static void GetOptionDeclaration(StringBuilderWrapper sb, CommandInfo commandInfo, PropInfo propInfo)
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
        AddSymbolToLookup(sb, commandInfo, propInfo, symbolName);
        sb.AppendLine($"""cmd.Add({symbolName});""");
        sb.AppendLine();

    }

    internal static void GetArgumentDeclaration(StringBuilderWrapper sb, CommandInfo commandInfo, PropInfo propInfo)
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
        AddSymbolToLookup(sb, commandInfo, propInfo, symbolName);
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

    private static void AddSymbolToLookup(StringBuilderWrapper sb, CommandInfo commandInfo, PropInfo propInfo, string symbolName)
    {
        sb.AppendLine($"""cliDataProvider.AddNameLookup((typeof({commandInfo.Name}), nameof({propInfo.Name})), {symbolName});""");
    }

    private static void CreateInstance(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""protected override {commandInfo.RootName} CreateInstance(DataValues dataValues)""");

        sb.OpenIf($"dataValues is not {commandInfo.Name}DataValues typedDataValues");
        sb.AppendLine("throw new InvalidOperationException(\"Internal error: passed incorrect data values\");");
        sb.CloseIf();

        var ctorArguments = commandInfo.SelfAndAncestorPropInfos.Select(p => $"typedDataValues.{p.Name}");
        sb.AppendLine();
        sb.Append($"return new {commandInfo.Name}({string.Join(", ", ctorArguments)});");
        sb.CloseMethod();

    }

    private static void CheckRequiredValues(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""protected override IEnumerable<ValidationFailure> CheckRequiredValues(DataValues dataValues)""");

        sb.OpenIf($"dataValues is not {commandInfo.Name}DataValues typedDataValues");
        sb.AppendLine("throw new InvalidOperationException(\"Internal error: passed incorrect data values\");");
        sb.CloseIf();

        var requiredValues = commandInfo.PropInfos.Where(p => p.IsRequiredForCli).ToList();
        sb.AppendLine($"var requiredFailures = new List<ValidationFailure?>();");

        foreach (var requiredValue in requiredValues)
        {
            var propName = requiredValue.Name;
            sb.AppendLine($"AddRequiredFailureIfNeeded<{requiredValue.TypeName}>(requiredFailures, !typedDataValues.{propName}.IsSet, nameof({propName}));");
        }

        sb.AppendLines([
            "return requiredFailures",
            "          .Where(x => x is not null)",
            "          .Select(x => x!);"
            ]);

        sb.CloseMethod();
    }

    private static void CreateDataValues(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""protected override DataValues<{commandInfo.RootName}> CreateDataValues()""");
        sb.AppendLine($"return new {commandInfo.Name}DataValues();");
        sb.CloseMethod();
    }
}

