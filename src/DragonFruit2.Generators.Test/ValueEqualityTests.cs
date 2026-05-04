namespace DragonFruit2.Generators.Test;

using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;

public class ValueEqualityTests
{
    [Fact]
    public void CliInfoSupportsValueEquality()
    {
        var cliInfo1 = CreateCliInfo();
        var cliInfo2 = CreateCliInfo();
        Assert.Equal(cliInfo1, cliInfo2);
    }

    private static CliInfo CreateCliInfo()
    {
        return new CliInfo
        {
            RootTypeNamespace = "TestNamespace",
            RootCommandName = "TestCommand",
            EntryPointNamespace = "TestEntryPoint"
        };
    }

    [Fact]
    public void CommandInfoSupportsValueEquality()
    {
        var commandInfo1 = CreateCommandInfo();
        var commandInfo2 = CreateCommandInfo();

        Assert.Equal(commandInfo1, commandInfo2);

        static CommandInfo CreateCommandInfo()
        {
            var commandInfo = new CommandInfo
            {
                Name = "TestCommand",
                NamespaceName = "TestNamespace",
                BaseTypeName = "BaseCommand",
                BaseTypeNamespace = "BaseCommand",
                Accessibility = "public"
            };
            commandInfo.Arguments.Add(
                CreatePropInfo("Name", "string")
            );

            commandInfo.Options.Add(CreatePropInfo("Age", "int"));
            return commandInfo;
        }
    }

    [Fact]
    public void PropInfoSupportsValueEquality()
    {
        var propInfo1 = CreatePropInfo("Name", "string");
        var propInfo2 = CreatePropInfo("Name", "string");

        Assert.Equal(propInfo1, propInfo2);

    }

    private static PropInfo CreatePropInfo(string name, string typeName)
    {
        return new PropInfo
        {
            Name = name,
            TypeName = typeName,
            ContainingTypeName = "TestCommand",
            IsValueType = false,
            NullableAnnotation = NullableAnnotation.None,
            HasRequiredModifier = false,
            Description = "Test description",
            HasArgumentAttribute = false,
            Position = 0,
            IsArgument = false,
            HasInitializer = false,
            InitializerText = "initializerText",
        };
    }

    [Fact]
    public void CommandNodeSupportsValueEquality()
    {
        var commandNode1 = CreateCommandNode();
        var commandNode2 = CreateCommandNode();
        Assert.Equal(commandNode1, commandNode2);

        static CommandNode CreateCommandNode()
        {
            var commandInfo = new CommandInfo
            {
                Name = "TestCommand",
                NamespaceName = "TestNamespace",
                BaseTypeName = "Object",
                BaseTypeNamespace = "System",
                Accessibility = "public"
            };

            var subCommandInfo = new CommandInfo
            {
                Name = "ChildCommand",
                NamespaceName = "TestNamespace",
                BaseTypeName = "TestCommand",
                BaseTypeNamespace = "TestNamespace",
                Accessibility = "public"
            };
            var commandNode = new CommandNode
            {
                CommandInfo = commandInfo,
                ParentCommandNode = null,
            };
            var subCommandNode = new CommandNode
            {
                CommandInfo = subCommandInfo,
                ParentCommandNode = commandNode,
            };
            commandNode.SubCommands.Add(subCommandNode);
            commandNode.SetRootCommandNode();
            subCommandNode.SetRootCommandNode();

            return commandNode;
        }
    }

    [Fact]
    public void CliInfoGroupSupportsValueEquality()
    {
        var cliInfoGroup1 = CreateCliInfoGroup();
        var cliInfoGroup2 = CreateCliInfoGroup();
        Assert.Equal(cliInfoGroup1, cliInfoGroup2);

        static CliInfoGroup CreateCliInfoGroup()
        {
            var cliInfoGroup = new CliInfoGroup(null)
            {
                EntryPointNamespace = "TestEntryPoint"
            };
            cliInfoGroup.CliInfos.Add(CreateCliInfo());
            return cliInfoGroup;
        }
    }
}
