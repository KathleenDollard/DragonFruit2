using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.Test;

public class CliInfoTheoryData
    : TheoryData<string, IEnumerable<string>>
{
    private void AddTheoryData(string name,  string[] consoleSources)
    {
        Add(name, consoleSources);
    }

    public CliInfoTheoryData()
    {
        AddTheoryData("MultipleNamespacesAndInvocationsSource",
                consoleSources:
                   [ """
                    using DragonFruit2;

                    // global namespace

                    var myArgsDataValues = Cli.ParseArgs<MyArgs>(args);
                    var myArgsDataValues2 = Cli.ParseArgs<MyArgs2>(args);

                    [CommandClass] class MyArgs{} 
                    [CommandClass] class MyArgs2{}
                    """,

                    """
                    using DragonFruit2;

                    namespace MyArgsNamespace;

                    [CommandClass] public class MyArgs3 
                    {}
                    """,

                    """
                    using DragonFruit2;

                    namespace MyNamespace;

                    public class OtherEntryPoints
                    {
                       public void OtherCalls(string[] args)
                       {
                          var myArgsDataValues = Cli.ParseArgs<MyArgsNamespace.MyArgs3>(args);
                       }
                    }
                    """,
                    ]
                );

    }

}

