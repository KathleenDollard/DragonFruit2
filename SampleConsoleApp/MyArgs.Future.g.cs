using DragonFruit2;
using System.CommandLine;

namespace SampleConsoleApp;

public partial class MyArgs : IArgs<MyArgs>
{
    public static System.CommandLine.Command CreateCli()
    {
        var rootCommand = new System.CommandLine.Command("Test");
        rootCommand.Add(new Option<string>("--name")
        {
            Description = "Your name",
            Required = true
        });
        rootCommand.Add(new Option<int>("--age")
        {
            Description = "Your Age"
        });
        rootCommand.Add(new Option<string>("--greeting")
        {
            Description = "Greeting message"
        });
        return rootCommand;
    }
    public void SetParsedValues(ParseResult result)
    {
        Name = result.GetValue<string>("--name");
        Age = result.GetValue<int>("--age");
        Greeting = result.GetValue<string>("greeting");
    }

    public static MyArgs Create(ParseResult parseResult)
    {
        var newArgs = new MyArgs()
        {
            Name = parseResult.GetValue<string>("--name"),
            Age = parseResult.GetValue<int>("--age"),
            Greeting = parseResult.GetValue<string>("--greeting")
        };
        return newArgs;
    }
}
