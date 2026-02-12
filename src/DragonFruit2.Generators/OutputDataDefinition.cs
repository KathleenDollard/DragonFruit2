namespace DragonFruit2.Generators;

public class OutputDataDefinition
{
    public static void GetClass(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        OpenClass(sb, commandInfo);

        Constructor(sb, commandInfo);
        Properties(sb, commandInfo);
        GetMemberDefinition(sb, commandInfo);
        Operate(sb, commandInfo);
        sb.AppendLine();

        sb.CloseClass();
    }

    internal static void OpenClass(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.AppendLine();
        sb.XmlSummary(" The data definition is available to data providers and are used for initialization.");
        sb.OpenClass($"public partial class {commandInfo.Name}DataDefinition : CommandDataDefinition<{commandInfo.RootName}>");
    }

    private static void Constructor(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenConstructor($"public {commandInfo.Name}DataDefinition(CommandDataDefinition? parentDataDefinition, CommandDataDefinition? rootDataDefinition)",
              $"base(parentDataDefinition, rootDataDefinition)");

        sb.AppendLine($"GetDataValues = () => new {commandInfo.Name}DataValues(this);");

        foreach (var optionInfo in commandInfo.Options)
        {
            sb.AppendLine($"{optionInfo.Name} = new OptionDataDefinition<{optionInfo.TypeName}>(this, nameof({optionInfo.Name}))");
            sb.OpenCurly();
            AddMemberInfo(sb, optionInfo);
            sb.CloseCurly(endStatement: true);
            AddValidation(sb, optionInfo);
            AddDefaults(sb, optionInfo);
            sb.AppendLine();
        }

        foreach (var argumentInfo in commandInfo.Arguments)
        {
            sb.AppendLine($"{argumentInfo.Name} = new ArgumentDataDefinition<{argumentInfo.TypeName}>(this, nameof({argumentInfo.Name}))");
            sb.OpenCurly();
            AddMemberInfo(sb, argumentInfo);
            sb.CloseCurly(endStatement: true);
            AddValidation(sb, argumentInfo);
            AddDefaults(sb, argumentInfo);
            sb.AppendLine();
        }

        foreach (var subcommandInfo in commandInfo.SubCommands)
        {
            AddSubcommandInfo(sb, subcommandInfo);
            sb.AppendLine();
        }

        sb.AppendLine("RegisterCustomizations();");

        sb.CloseConstructor();

        static void AddMemberInfo(StringBuilderWrapper sb, PropInfo propInfo)
        {
            sb.AppendLine($"""DataType = typeof({propInfo.TypeName}), """);
            sb.AppendLine($"""IsRequired = {sb.CSharpString(propInfo.IsRequiredForCli)}, """);
        }

        static void AddSubcommandInfo(StringBuilderWrapper sb, CommandInfo subcommandInfo)
        {
            sb.AppendLine($"Add(new {subcommandInfo.Name}.{subcommandInfo.Name}DataDefinition(this, this.RootDataDefinition)");
            sb.OpenCurly();
            sb.CloseCurly(closeParens: true, endStatement: true);

        }
   
        static void AddValidation(StringBuilderWrapper sb, PropInfo propInfo)
        {
            foreach (var validatorInfo in propInfo.Validators)
            {
                sb.AppendLine($"{propInfo.Name}.RegisterValidator(new {validatorInfo.ValidatorName}<{propInfo.TypeName}>({propInfo.Name}.DefinitionName, 0));");
            }
        }
        
        static void AddDefaults(StringBuilderWrapper sb, PropInfo propInfo)
        {
            // not yet implemented
        }


    }



    private static void Properties(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        foreach (var propInfo in commandInfo.PropInfos)
        {
            sb.AppendLine($"public OptionDataDefinition<{propInfo.TypeName}> {propInfo.Name} {{ get; }}");

        }
    }

    private static void GetMemberDefinition(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod("protected override MemberDataDefinition? GetMemberDefinition(string memberName)");

        sb.AppendLine("return memberName switch");
        sb.OpenCurly();
        foreach (var propInfo in commandInfo.PropInfos)
        {
            sb.AppendLine($"nameof({propInfo.Name}) => {propInfo.Name},");
        }
        sb.CloseCurly(endStatement: true);

        sb.CloseMethod();
    }


    private static void Operate(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod("public override IEnumerable<TReturn> Operate<TReturn> (IOperationOnMemberDefinition<TReturn> operationContainer)");
        sb.AppendLine($"var retValues = new TReturn[{commandInfo.PropInfos.Count()}];");

        var i = 0;
        foreach (var propInfo in commandInfo.PropInfos)
        {
            sb.AppendLine($"retValues[{i}] = operationContainer.Operate({propInfo.Name});");
            i++;
        }

        sb.Return("retValues");
        sb.CloseMethod();
    }
}