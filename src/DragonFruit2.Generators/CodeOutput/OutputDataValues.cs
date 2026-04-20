using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.CodeOutput;

internal class OutputDataValues
{
    internal static void GetClass(StringBuilderWrapper sb, CommandNode commandNode)
    {
        OpenClass( commandNode , sb);

        Constructor(sb,  commandNode );
        Operate(sb,  commandNode );
        sb.AppendLine();
        Fields(sb,  commandNode );
        Properties(sb,  commandNode );
        CreateInstance(sb,  commandNode );

        sb.CloseClass();
    }

    private static void OpenClass( CommandNode  commandNode, StringBuilderWrapper sb)
    {
        sb.AppendLine();
        sb.OpenClass($"public class { commandNode.CommandInfo.Name}DataValues : DataValues<{ commandNode.RootCommandName}>");
    }

    private static void Constructor(StringBuilderWrapper sb,  CommandNode  commandNode)
    {
        sb.OpenConstructor($"public { commandNode.CommandInfo.Name}DataValues({ commandNode.CommandInfo.Name}DataDefinition commandDefinition)",
            "base(commandDefinition)");
        foreach (var propInfo in  commandNode.CommandInfo.PropInfos)
        {
            sb.AppendLine($"{propInfo.Name} = DataValue<{propInfo.TypeName}>.Create(nameof({propInfo.Name}), argsType, this, commandDefinition.{propInfo.Name});");
            sb.AppendLine($"Add({propInfo.Name});");
        }
        sb.CloseConstructor();
    }

    private static void Operate(StringBuilderWrapper sb,  CommandNode  commandNode)
    {
        sb.OpenMethod($" public override bool Operate<TReturn>(IOperateOnDataValue<{ commandNode.RootCommandNode}, TReturn> operationContainer)");
        sb.OpenTry();
        foreach (var propInfo in  commandNode.SelfAndAncestorPropInfos)
        {
            sb.AppendLine($"operationContainer.TryOperate({propInfo.Name}, operationContainer, out var _);");
        }
        sb.Return("true");
        sb.CloseTryAndOpenCatch();
        sb.AppendLine($$"""Diagnostic failure = new(DiagnosticId.UnexpectedException.ToValidationIdString(), DiagnosticSeverity.Error, $"An unexpected error occurred while operating on data values in the {operationContainer.OperationName}");""");
        sb.AppendLine("operationContainer.Result.AddDiagnostic(failure);");
        sb.Return("false");
        sb.CloseCatch();
        sb.CloseMethod();
    }

    private static void Fields(StringBuilderWrapper sb,  CommandNode  commandNode)
    {
        sb.AppendLine($"private Type argsType = typeof({ commandNode.CommandInfo.Name});");
    }

    private static void Properties(StringBuilderWrapper sb,  CommandNode  commandNode)
    {
        foreach (var propInfo in  commandNode.SelfAndAncestorPropInfos)
        {
            sb.AppendLine($"public DataValue<{propInfo.TypeName}> {propInfo.Name} {{ get; }}");
        }
    }

    private static void CreateInstance(StringBuilderWrapper sb,  CommandNode  commandNode)
    {
        sb.OpenMethod($"""protected override { commandNode.CommandInfo.Name} CreateInstance()""");
        var ctorArguments =  commandNode.SelfAndAncestorPropInfos.Select(p => p.Name);
        sb.Append($"return new { commandNode.CommandInfo.Name}({string.Join(", ", ctorArguments)});");
        sb.CloseMethod();

    }
}
