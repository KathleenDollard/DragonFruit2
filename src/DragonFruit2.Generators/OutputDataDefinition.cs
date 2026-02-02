namespace DragonFruit2.Generators;

public class OutputDataDefinition
{
    public static void GetClass(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        OpenClass(commandInfo, sb);

        Constructor(sb, commandInfo);
        sb.AppendLine();

        sb.CloseClass();
    }

    internal static void OpenClass(CommandInfo commandInfo, StringBuilderWrapper sb)
    {
        sb.AppendLine();
        sb.XmlSummary(" The data definition is available to data providers and are used for initialization.");
        sb.OpenClass($"internal class {commandInfo.Name}DataDefinition : CommandDataDefinition<{commandInfo.Name}>");
    }
    private static void Constructor(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenConstructor($"""public {commandInfo.Name}DataDefinition({commandInfo.RootName} rootArgs)""",
              $"""base(rootArgs)""");

        foreach (var optionInfo in commandInfo.Options)
        {
            sb.AppendLine($"""Add(new OptionDataDefinition""");
            sb.OpenCurly();
            AddMemberInfo(sb, optionInfo);
            sb.CloseCurly(closeParens: true, endStatement: true);
        }

        foreach (var argumentInfo in commandInfo.Arguments)
        {
            sb.AppendLine($"""Add(new OptionDataDefinition""");
            sb.OpenCurly();
            AddMemberInfo(sb, argumentInfo);
            sb.CloseCurly(closeParens: true, endStatement: true);
        }
        sb.CloseConstructor();

        static void AddMemberInfo(StringBuilderWrapper sb, PropInfo propInfo)
        {
            sb.AppendLine($"""FullName = typeof({propInfo.ContainingTypeName}).FullName + nameof({propInfo.Name}), """);
            sb.AppendLine($"""DataType = typeof({propInfo.TypeName}), """);
            sb.AppendLine($"""IsRequired = {sb.CSharpString(propInfo.IsRequiredForCli)}, """);
        }
    }

}