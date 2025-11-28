using System.CommandLine;
using System.Runtime.InteropServices;

namespace DragonFruit2;

public interface IArgs<T>
{
    public abstract static System.CommandLine.Command CreateCli();

    public abstract static T Create(ParseResult parseResult); 
}
