namespace DragonFruit2;

public static class Cli
{

    /// <summary>
    /// Parses CLI arguments to fill the specified args type. 
    /// </summary>
    /// <remarks>
    /// The args class specified as the type argument must be public.
    /// <br/>
    /// You may need to build after editing this line.
    /// </remarks>
    /// <typeparam name="TRootArgs">The type containing the CLI definition</typeparam>
    /// <param name="args">Optionaly pass the commandline args</param>
    /// <returns>A Result instance containing the hydrated args or error messages.</returns>
    public static Result<TRootArgs> ParseArgs<TRootArgs>(string[]? args = null)
        where TRootArgs : ArgsRootBase<TRootArgs>
    {
        throw new InvalidOperationException("This method is a stub for source generation. You either called `DragonFruit2.Cli.TryParse` instead of an import for DragonFruit2 and `Cli.TryParse' or there is a problem with source generation.");
    }

    /// <summary>
    /// Attempts to parses CLI arguments and fill the specified args type.
    /// </summary>
    /// <remarks>
    /// The args class specified as the type argument must be public.
    /// <br/>
    /// You may need to build after editing this line.
    /// </remarks>
    /// <typeparam name="TRootArgs">The type containing the CLI definition</typeparam>
    /// <param name="result">An out parameter that contains an instance of the requested class and supporting data, such as a suggested CLI return value.</param>
    /// <param name="args">Optionaly pass the commandline args</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool TryParseArgs<TRootArgs>(out Result<TRootArgs> result, string[]? args = null)
            where TRootArgs : ArgsRootBase<TRootArgs>
    {
        throw new InvalidOperationException("This method is a stub for source generation. You either called `DragonFruit2.Cli.TryParse` instead of an import for DragonFruit2 and `Cli.TryParse' or there is a problem with source generation.");
    }

    public static bool TryExecute<TRootArgs>(out Result<TRootArgs> result, string[]? args = null)
        where TRootArgs : ArgsRootBase<TRootArgs>
    {
        throw new InvalidOperationException("This method is a stub for source generation. You either called `DragonFruit2.Cli.TryParse` instead of an import for DragonFruit2 and `Cli.TryParse' or there is a problem with source generation.");
    }

}
