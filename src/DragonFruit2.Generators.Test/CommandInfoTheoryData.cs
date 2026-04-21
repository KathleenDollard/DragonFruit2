using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.Test;

public class CommandInfoTheoryData : TheoryData<string, string, string>
{
    private void AddTheoryData(string name, string argsSource, string consoleSource)
    {
        Add(name, argsSource, consoleSource);
    }

    public CommandInfoTheoryData()
    {
        AddTheoryData("MultipleCallsOneNamespace",
                argsSource:
                    """
                    using DragonFruit2;


                    namespace MyNamespace
                    {
                        [CommandClass]
                        public partial class MyArgs : CommandRootBase<MyArgs>
                        {
                            public required string Name { get; set; }
                        }

                        [CommandClass]
                        public partial class MorningGreetingArgs : MyArgs
                        {
                        }

                        [CommandClass]
                        public partial class EveningGreetingArgs : MyArgs
                        {
                            public int Age { get; init; } = 1;
                        }
                        [CommandClass]
                        public partial class Bar : EveningGreetingArgs
                        {
                        }
                    }
                    """,
                consoleSource: TestHelpers.EmptyConsoleAppCodeWithArgsMyNamespace);

    }

}

