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
            writer.WriteMember(item, item.FullName, "Name");
            writer.WriteMember(item, item.ParentCommandNode?.FullName, "ParentCommand");
            writer.WriteMember(item, item.RootCommandNode?.FullName, "RootCommand");

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
}
