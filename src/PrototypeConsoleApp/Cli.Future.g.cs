using DragonFruit2;
using SampleConsoleApp;

public class Cli 
{
    public static DataValues<MyArgs> ParseArgs<TRootArgs>(string[]? args = null)
        where TRootArgs : IArgs<TRootArgs>
    {     
        return DragonFruit2.Cli.ParseArgs<MyArgs>(new MyArgs.MyArgsBuilder(), args);
    }
}
