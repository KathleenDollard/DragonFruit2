namespace DragonFruit2;

public static class Cli
{

    /// <summary>
    /// Parses CLI arguments to fill the specified CommandClass as part of a Resut object
    /// </summary>
    /// <remarks>
    /// The Command class specified as the type argument must be public.
    /// <br/>
    /// You may need to build after editing this line.
    /// </remarks>
    /// <typeparam name="TRootCommand">The type containing the CLI definition</typeparam>
    /// <param name="args">Optionaly pass the commandline args</param>
    /// <returns>A Result instance containing the hydrated CommandClass or error messages.</returns>
    public static Result<TRootCommand> ParseArgs<TRootCommand>(string[]? args = null)
    {
        throw new InvalidOperationException("This method is a stub for source generation. You either called `DragonFruit2.Cli.TryParse` instead of an import for DragonFruit2 and `Cli.TryParse' or there is a problem with source generation.");
    }

    /// <summary>
    /// Attempts to parse CLI arguments to fill the specified CommandClass as part of a Resut object
    /// </summary>
    /// <remarks>
    /// The Command class specified as the type argument must be public.
    /// <br/>
    /// You may need to build after editing this line.
    /// </remarks>
    /// <typeparam name="TRootCommand">The type containing the CLI definition</typeparam>
    /// <param name="result">An out parameter that contains an instance of the requested class and supporting data, such as a suggested CLI return value.</param>
    /// <param name="args">Optionaly pass the commandline args</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool TryParseArgs<TRootCommand>(out Result<TRootCommand> result, string[]? args = null) 
    {
        throw new InvalidOperationException("This method is a stub for source generation. You either called `DragonFruit2.Cli.TryParse` instead of an import for DragonFruit2 and `Cli.TryParse' or there is a problem with source generation.");
    }

    public static bool TryExecute<TRootCommand>(out Result<TRootCommand> result, string[]? args = null)
    {
        throw new InvalidOperationException("This method is a stub for source generation. You either called `DragonFruit2.Cli.TryParse` instead of an import for DragonFruit2 and `Cli.TryParse' or there is a problem with source generation.");
    }

}
