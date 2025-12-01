using DragonFruit2.Common.Net8;
using System.CommandLine;

namespace DragonFruit2;

public abstract class CliArgs : Args
{
    public static CliDataProvider SetCliDataProvider<T>(ParseResult parseResult) where T : CliArgs
    {
        var cliProvider = DataProviders.OfType<CliDataProvider>().FirstOrDefault();
        if (cliProvider is null)
        {
            cliProvider = new CliDataProvider();
            AddDataProvider( cliProvider, 0);
        }
        cliProvider.ParseResult = parseResult;
        return cliProvider;
    }

}
