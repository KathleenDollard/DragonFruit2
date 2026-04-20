using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.CodeOutput;

public class OutputDataDefinition
{
    public static void GetClass(StringBuilderWrapper sb, CommandNode commandNode)
    {
        OpenClass(sb, commandNode);

        Constructor(sb, commandNode);
        Properties(sb, commandNode);
        GetMemberDefinition(sb, commandNode);
        Operate(sb, commandNode);
        sb.AppendLine();

        sb.CloseClass();
    }

    internal static void OpenClass(StringBuilderWrapper sb, CommandNode commandNode)
    {
        sb.AppendLine();
        sb.XmlSummary(" The data definition is available to data providers and are used for initialization.");
        sb.OpenClass($"public partial class {commandNode .CommandInfo.Name}DataDefinition : CommandDataDefinition<{commandNode.RootCommandName}>");
    }

    private static void Constructor(StringBuilderWrapper sb, CommandNode commandNode)
    {
        sb.OpenConstructor($"public {commandNode.CommandInfo.Name}DataDefinition(CommandDataDefinition? parentDataDefinition, CommandDataDefinition? rootDataDefinition)",
              $"base(parentDataDefinition, rootDataDefinition)");

        sb.AppendLine($"GetDataValues = () => new {commandNode.CommandInfo.Name}DataValues(this);");

        foreach (var optionInfo in commandNode.CommandInfo.Options)
        {
            sb.AppendLine($"{optionInfo.Name} = new OptionDataDefinition<{optionInfo.TypeName}>(this, nameof({optionInfo.Name}))");
            sb.OpenCurly();
            AddMemberInfo(sb, optionInfo);
            sb.CloseCurly(endStatement: true);
            AddValidation(sb, optionInfo);
            AddDefaults(sb, optionInfo);
            sb.AppendLine();
        }

        foreach (var argumentInfo in commandNode.CommandInfo.Arguments)
        {
            sb.AppendLine($"{argumentInfo.Name} = new ArgumentDataDefinition<{argumentInfo.TypeName}>(this, nameof({argumentInfo.Name}))");
            sb.OpenCurly();
            AddMemberInfo(sb, argumentInfo);
            sb.CloseCurly(endStatement: true);
            AddValidation(sb, argumentInfo);
            AddDefaults(sb, argumentInfo);
            sb.AppendLine();
        }

        if (commandNode?.SubCommands is not null)
        {
            foreach (var subcommandInfo in commandNode.SubCommands)
            {
                AddSubcommandInfo(sb, subcommandInfo);
                sb.AppendLine();
            }
        }

        sb.AppendLine("RegisterCustomizations();");

        sb.CloseConstructor();

        static void AddMemberInfo(StringBuilderWrapper sb, PropInfo propInfo)
        {
            sb.AppendLine($"""DataType = typeof({propInfo.TypeName}), """);
            sb.AppendLine($"""IsRequired = {sb.CSharpString(propInfo.IsRequiredForCli)}, """);
        }

        static void AddSubcommandInfo(StringBuilderWrapper sb, CommandNode subcommandInfo)
        {
            sb.AppendLine($"Add(new {subcommandInfo.CommandInfo.Name}.{subcommandInfo.CommandInfo.Name}DataDefinition(this, this.RootDataDefinition)");
            sb.OpenCurly();
            sb.CloseCurly(closeParens: true, endStatement: true);

        }
   
        static void AddValidation(StringBuilderWrapper sb, PropInfo propInfo)
        {
            foreach (var validatorInfo in propInfo.Validators)
            {
                sb.AppendLine($"{propInfo.Name}.RegisterValidator(new {validatorInfo.ValidatorTypeName}<{propInfo.TypeName}>({propInfo.Name}.DefinitionName, 0));");
            }
        }
        
        static void AddDefaults(StringBuilderWrapper sb, PropInfo propInfo)
        {
            // not yet implemented
        }


    }



    private static void Properties(StringBuilderWrapper sb, CommandNode commandNode)
    {
        foreach (var propInfo in commandNode.CommandInfo.PropInfos)
        {
            sb.AppendLine($"public OptionDataDefinition<{propInfo.TypeName}> {propInfo.Name} {{ get; }}");

        }
    }

    private static void GetMemberDefinition(StringBuilderWrapper sb, CommandNode commandNode)
    {
        sb.OpenMethod("protected override MemberDataDefinition? GetMemberDefinition(string memberName)");

        sb.AppendLine("return memberName switch");
        sb.OpenCurly();
        foreach (var propInfo in commandNode.CommandInfo.PropInfos)
        {
            sb.AppendLine($"nameof({propInfo.Name}) => {propInfo.Name},");
        }
        sb.CloseCurly(endStatement: true);

        sb.CloseMethod();
    }


    private static void Operate(StringBuilderWrapper sb, CommandNode commandNode)
    {
        sb.OpenMethod("public override IEnumerable<TReturn> Operate<TReturn> (IOperationOnMemberDefinition<TReturn> operationContainer)");
        sb.AppendLine($"var retValues = new TReturn[{commandNode.CommandInfo.PropInfos.Count()}];");

        var i = 0;
        foreach (var propInfo in commandNode.CommandInfo.PropInfos)
        {
            sb.AppendLine($"retValues[{i}] = operationContainer.Operate({propInfo.Name});");
            i++;
        }

        sb.Return("retValues");
        sb.CloseMethod();
    }
}