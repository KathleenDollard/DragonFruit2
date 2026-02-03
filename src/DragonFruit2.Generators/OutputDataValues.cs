namespace DragonFruit2.Generators;

internal class OutputDataValues
{
    internal static void GetClass(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        OpenClass(commandInfo, sb);

        SetDataValues(sb, commandInfo);
        sb.AppendLine();
        Fields(sb, commandInfo);
        Properties(sb, commandInfo);
        CreateInstance(sb, commandInfo);

        sb.CloseClass();
    }

    private static void OpenClass(CommandInfo commandInfo, StringBuilderWrapper sb)
    {
        sb.AppendLine();
        sb.OpenClass($"public class {commandInfo.Name}DataValues : DataValues<{commandInfo.RootName}>");
    }

    private static void SetDataValues(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"public override void SetDataValues(DataProvider<{commandInfo.RootName}> dataProvider)");
        foreach (var propInfo in commandInfo.SelfAndAncestorPropInfos)
        {
            sb.AppendLine($"dataProvider.TrySetDataValue((typeof({propInfo.ContainingTypeName}), nameof({propInfo.Name})), {propInfo.Name});");
        }
        sb.CloseMethod();
    }

    private static void Fields(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.AppendLine($"private Type argsType = typeof({commandInfo.Name});");
    }

    private static void Properties(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        foreach (var propInfo in commandInfo.SelfAndAncestorPropInfos)
        {
            sb.AppendLine($"public DataValue<{propInfo.TypeName}> {propInfo.Name} {{ get; }} = DataValue<{propInfo.TypeName}>.Create(nameof({propInfo.Name}), typeof({commandInfo.Name}));");
        }
    }

    private static void CreateInstance(StringBuilderWrapper sb, CommandInfo commandInfo)
    {
        sb.OpenMethod($"""protected override {commandInfo.Name} CreateInstance()""");
        var ctorArguments = commandInfo.SelfAndAncestorPropInfos.Select(p => p.Name);
        sb.Append($"return new {commandInfo.Name}({string.Join(", ", ctorArguments)});");
        sb.CloseMethod();

    }
}
