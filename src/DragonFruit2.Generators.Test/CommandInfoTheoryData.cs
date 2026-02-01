namespace DragonFruit2.Generators.Test;

public class CommandInfoTheoryData : TheoryData<string, string, string, CommandInfo>
{
    private void AddTheoryData(string name, string argsSource, string consoleSource, CommandInfo commandInfo)
    {
        Add(name, argsSource, consoleSource, commandInfo);
    }

    public CommandInfoTheoryData()
    {
        AddTheoryData("Empty args class",
            argsSource:
                """
                public partial class MyArgs : ArgsRootBase<MyArgs>
                { }
                """,
            consoleSource: TestHelpers.EmptyConsoleAppCode,
            commandInfo:
                new CommandInfo()
                {
                    Name = "MyArgs",
                    RootName = "MyArgs",
                    ArgsAccessibility = "public"
                });

        AddTheoryData("Empty args struct",
           argsSource:
                """
                public partial struct MyArgs : ArgsRootBase<MyArgs>
                { }
                """,
           consoleSource: TestHelpers.EmptyConsoleAppCode,
           commandInfo:
               new CommandInfo()
               {
                   Name = "MyArgs",
                   RootName = "MyArgs",
                   IsStruct = true,
                   ArgsAccessibility = "public"
               });

        AddTheoryData("Args w/ namespace (curly)",
           argsSource:
                """
                namespace MyNamespace
                {
                    public partial class MyArgs : ArgsRootBase<MyArgs>
                    { }
                }
                """,
            consoleSource: TestHelpers.EmptyConsoleAppCodeWithArgsMyNamespace,
            commandInfo:
                new CommandInfo()
                {
                    Name = "MyArgs",
                    RootName = "MyArgs",
                    NamespaceName = "MyNamespace",
                    ArgsAccessibility = "public"
                });

        AddTheoryData("Duplicate ParseArgs calls",
            argsSource:
                """
                        public partial class MyArgs : ArgsRootBase<MyArgs>
                        { }
                        """,
            consoleSource: TestHelpers.ConsoleAppWithDuplicateCall,
            commandInfo:
                new CommandInfo()
                {
                    Name = "MyArgs",
                    RootName = "MyArgs",
                    ArgsAccessibility = "public"
                });

        AddTheoryData("Two different ParseArgs calls",
             argsSource:
                 """
                        public partial class MyArgs : ArgsRootBase<MyArgs>
                        { }

                        public partial class MyOtherArgs : ArgsRootBase<MyOtherArgs>
                        { }
                        """,
            consoleSource: TestHelpers.ConsoleAppWithTwoDifferentCalls,
            commandInfo:
                new CommandInfo()
                {
                    Name = "MyArgs",
                    RootName = "MyArgs",
                    ArgsAccessibility = "public"
                });

        AddTheoryData("TryParse",
            argsSource:
                """
                        public partial class MyArgs : ArgsRootBase<MyArgs>
                        { }
                        """,
            consoleSource: TestHelpers.ConsoleAppWithTryParseCall,
            commandInfo:
                new CommandInfo()
                {
                    Name = "MyArgs",
                    RootName = "MyArgs",
                    ArgsAccessibility = "public"
                });

        AddTheoryData("TryExecute",
            argsSource:
                """
                        public partial class MyArgs : ArgsRootBase<MyArgs>
                        { }
                        """,
            consoleSource: TestHelpers.ConsoleAppWithTryExecuteCall,
            commandInfo:
                new CommandInfo()
                {
                    Name = "MyArgs",
                    RootName = "MyArgs",
                    ArgsAccessibility = "public"
                });

        AddTheoryData("SubCommands",
                argsSource:
                   """
                    using DragonFruit2.Validators;

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
                            [GreaterThan(0)]
                            public int Age { get; init; } = 1;
                        }
                    }
                    """,
                consoleSource: TestHelpers.EmptyConsoleAppCodeWithArgsMyNamespace,
                commandInfo: GetSubCommandCommandInfo());

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

        static CommandInfo GetSubCommandCommandInfo()
        {
            var subCommandsCommandInfo = new CommandInfo()
            {
                Name = "MyArgs",
                RootName = "MyArgs",
                NamespaceName = "MyNamespace",
                ArgsAccessibility = "public"
            };
            subCommandsCommandInfo.Options.Add(new PropInfo()
            {
                Name = "Name",
                TypeName = "string",
                ContainingTypeName = "MyArgs",
                HasRequiredModifier = true,
            });
            subCommandsCommandInfo.SubCommands.Add(
                        new CommandInfo()
                        {
                            Name = "MorningGreetingArgs",
                            RootName = "MyArgs",
                            NamespaceName = "MyNamespace",
                            ArgsAccessibility = "public"
                        });
            var subCommandsEveningCommandInfo = new CommandInfo()
            {
                Name = "EveningGreetingArgs",
                RootName = "MyArgs",
                NamespaceName = "MyNamespace",
                ArgsAccessibility = "public"
            };
            subCommandsEveningCommandInfo.Options.Add(new PropInfo()
            {
                Name = "Age",
                TypeName = "int",
                ContainingTypeName = "MyArgs",
                HasRequiredModifier = false,
            });
            subCommandsCommandInfo.SubCommands.Add(subCommandsEveningCommandInfo);

            return subCommandsCommandInfo;
        }

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

