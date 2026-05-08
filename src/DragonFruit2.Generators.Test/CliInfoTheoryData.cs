using DragonFruit2.Generators.Metadata;

namespace DragonFruit2.Generators.Test;

[Serializable]
public class CliInfoTheoryData
    : TheoryData<string, IEnumerable<string>>
{
    private void AddTheoryData(string name,  string[] sources)
    {
        Add(name, sources);
    }

    public CliInfoTheoryData()
    {
        AddTheoryData("MultipleNamespacesAndInvocationsSource",
                sources:
                   [ """
                    using DragonFruit2;

                    // global namespace

                    var myArgsDataValues = Cli.ParseArgs<MyCommand>(args);
                    var myArgsDataValues2 = Cli.ParseArgs<MyCommand2>(args);

                    [CommandClass] class MyCommand{} 
                    [CommandClass] class MyCommand2{}
                    """,

                    """
                    using DragonFruit2;

                    namespace MyCommandNamespace;

                    [CommandClass] public class MyCommand3 
                    {}
                    """,

                    """
                    using DragonFruit2;

                    namespace MyCommandNamespace2;

                    [CommandClass] public class MyCommand4
                    {}
                    """,

                    """
                    using DragonFruit2;

                    namespace MyNamespace;

                    public class OtherEntryPoints
                    {
                       public void OtherCalls(string[] args)
                       {
                          var myArgsDataValues = Cli.ParseArgs<MyCommandNamespace.MyCommand3>(args);
                          var myArgsDataValues = Cli.ParseArgs<MyCommandNamespace2.MyCommand4>(args);
                       }
                    }
                    """,
                    ]
                );

    }

}

