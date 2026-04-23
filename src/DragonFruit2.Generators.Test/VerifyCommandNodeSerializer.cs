using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.Test;


public class VerifyCommandNodeEnumerableSerializer :
        WriteOnlyJsonConverter<IEnumerable<CommandNode>>
{
    public override void Write(VerifyJsonWriter writer, IEnumerable<CommandNode> collection)
    {
        // Custom logic to simplify or filter the collection for snapshots
        writer.WriteStartArray();
        foreach (var item in collection)
        {
            writer.WriteStartObject();
            writer.WriteMember(item, item.Name, "Name");
            writer.WriteMember(item, item.ParentCommandFullName, "ParentCommandFullName");
            writer.WriteMember(item, item.RootCommandFullName, "RootCommandName");

            writer.WriteMember(item, item.SubCommands, "SubCommands");
            //writer.WriteStartArray();
            //foreach (var commandNode in item.SubCommands)
            //{
            //    WriteCommandNode(writer, commandNode);
            //}
            //writer.WriteEndArray();

            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }

    private static void WriteCommandNode(VerifyJsonWriter writer, CommandNode commandNode)
    {
        writer.WriteStartObject();
        writer.WriteMember(commandNode, commandNode.Name, "Name");
        writer.WriteMember(commandNode, commandNode.ParentCommandFullName, "ParentCommandFullName");
        writer.WriteMember(commandNode, commandNode.RootCommandFullName, "RootCommandName");
        //if (!commandNode.SubCommands.Any()) return;

        //foreach (var subCommand in commandNode.SubCommands)
        //{
        //    WriteCommandNode(writer, subCommand);
        //}
        writer.WriteEndObject();
    }
}
