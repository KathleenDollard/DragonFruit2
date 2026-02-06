using System.ComponentModel;
using System.Reflection;
using System.Xml;

namespace DragonFruit2.Generators;

public class OutputDataDefinition
{
    public static void GetClass(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        OpenClass(sb, commandInfo);

        Constructor(sb, commandInfo);
        Properties(sb, commandInfo);
        CreateMembers(sb, commandInfo);
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
            sb.AppendLine($"Add({optionInfo.Name});");
        }

        foreach (var argumentInfo in commandInfo.Arguments)
        {
            sb.AppendLine($"{argumentInfo.Name} = new ArgumentDataDefinition<{argumentInfo.TypeName}>(this, nameof({argumentInfo.Name}))");
            sb.OpenCurly();
            AddMemberInfo(sb, argumentInfo);
            sb.CloseCurly(endStatement: true);
            sb.AppendLine($"Add({argumentInfo.Name});");
        }

        foreach (var subcommandInfo in commandInfo.SubCommands)
        {
            AddSubcommandInfo(sb, subcommandInfo);
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
    }

    private static void Properties(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        foreach (var propInfo in commandInfo.PropInfos)
        {
            sb.AppendLine($"public OptionDataDefinition<{propInfo.TypeName}> {propInfo.Name} {{ get; }}");

        }
    }

    private static void CreateMembers(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod("public override IEnumerable<TReturn> CreateMembers<TReturn>(ICreatesMembers<TReturn> dataProvider)");
        sb.Return("new List<TReturn>", true);
        sb.OpenCurly();
        foreach (var option in commandInfo.Options.Concat(commandInfo.Arguments))
        {
            sb.AppendLine($"dataProvider.CreateMember<{option.TypeName}>(this, nameof({option.Name})),");
        }
        sb.CloseCurly(endStatement: true);
        sb.CloseMethod();
    }

}