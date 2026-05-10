using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.Test;

public class CommandInfoTheoryData : TheoryData<string, string, string>
{
    private void AddTheoryData(string name, string commandSource, string consoleSource)
    {
        Add(name, commandSource, consoleSource);
    }

    public CommandInfoTheoryData()
    {
        AddTheoryData("DeepSubCommands",
                commandSource:
                    """
                    using DragonFruit2;

                    namespace MyNamespace
                    {
                        [CommandClass]
                        public partial class MyCommand : CommandClass
                        {
                            public required string Name { get; set; }
                        }

                        [CommandClass]
                        public partial class MorningGreetingCommand : MyCommand
                        {
                        }

                        [CommandClass]
                        public partial class EveningGreetingCommand : MyCommand
                        {
                            public int Age { get; init; } = 1;
                        }
                        [CommandClass]
                        public partial class Bar : EveningGreetingCommand
                        {
                        }
                    }

                    """,
                consoleSource: TestHelpers.EmptyConsoleAppCodeWithMyNamespace);

    }

}



