namespace DragonFruit2.Generators.Test;

public class CommandInfoTheoryData : TheoryData<string, string, string, CommandInfo>
{
    private void AddTheoryData(string name, string argsSource, string consoleSource, CommandInfo commandInfo)
    {
        Add(name, argsSource, consoleSource, commandInfo);
    }

    public CommandInfoTheoryData()
    {
        AddTheoryData("DeepSubCommands",
                argsSource:
                    """
                    namespace MyNamespace
                    {
                        public partial class MyArgs : ArgsRootBase<MyArgs>
                        {
                            public required string Name { get; set; }
                        }

                        public partial class MorningGreetingArgs : MyArgs
                        {
                        }

                        public partial class EveningGreetingArgs : MyArgs
                        {
                            public int Age { get; init; } = 1;
                        }
                        public partial class Bar : EveningGreetingArgs
                        {
                        }
                    }
                    """,
                consoleSource: TestHelpers.EmptyConsoleAppCodeWithArgsMyNamespace,
                commandInfo: GetDeepSubCommandsCommandInfo());

        static CommandInfo GetDeepSubCommandsCommandInfo()
        {
            var deepSubCommandInfo = new CommandInfo()
            {
                Name = "MyArgs",
                RootName = "MyArgs",
                NamespaceName = "MyNamespace",
                ArgsAccessibility = "public"
            };
            deepSubCommandInfo.Options.Add(new PropInfo()
            {
                Name = "Name",
                TypeName = "string",
                ContainingTypeName = "MyArgs",
                HasRequiredModifier = true,
            });
            deepSubCommandInfo.SubCommands.Add(
                        new CommandInfo()
                        {
                            Name = "MorningGreetingArgs",
                            RootName = "MyArgs",
                            NamespaceName = "MyNamespace",
                            ArgsAccessibility = "public"
                        });
            var deepSubCommandsEveningCommandInfo = new CommandInfo()
            {
                Name = "EveningGreetingArgs",
                RootName = "MyArgs",
                NamespaceName = "MyNamespace",
                ArgsAccessibility = "public"
            };
            deepSubCommandsEveningCommandInfo.Options.Add(new PropInfo()
            {
                Name = "Age",
                TypeName = "int",
                ContainingTypeName = "MyArgs",
                HasRequiredModifier = false,
            });
            var deeperSubCommandsEveningCommandInfo = new CommandInfo()
            {
                Name = "Bar",
                RootName = "MyArgs",
                NamespaceName = "MyNamespace",
                ArgsAccessibility = "public"
            };
            deepSubCommandsEveningCommandInfo.SubCommands.Add(deeperSubCommandsEveningCommandInfo);
            deepSubCommandInfo.SubCommands.Add(deepSubCommandsEveningCommandInfo);
            return deepSubCommandInfo;
        }
    }

}

