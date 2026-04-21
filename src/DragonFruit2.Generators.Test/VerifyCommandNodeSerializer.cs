using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.Test;

public class VerifyCommandNodeSerializer :
        WriteOnlyJsonConverter<CommandNode>
{
    public override void Write(VerifyJsonWriter writer, CommandNode commandNode)
    {
        writer.WriteMember(commandNode, commandNode.Name, "Name");
        writer.WriteMember(commandNode, commandNode.ParendCommandFullName, "ParendCommandName");
        writer.WriteMember(commandNode, commandNode.RootCommandFullName, "ParendCommandName");

    }
}
