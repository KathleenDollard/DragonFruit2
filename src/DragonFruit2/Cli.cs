namespace DragonFruit2;

public static class Cli
{

    /// <summary>
    /// </summary>
    /// <typeparam name="TArgs">The type containing the CLI definition</typeparam>
    /// <param name="args">Optionaly pass the commandline args</param>
    /// <returns></returns>
    public static TArgs? ParseArgs<TArgs>(string[]? args = null)
        where TArgs : IArgs<TArgs>
    {
        args ??= Environment.GetCommandLineArgs();

        var argsObject = new Builder<TArgs>().ParseArgs(args);


        return argsObject;

    }
}
