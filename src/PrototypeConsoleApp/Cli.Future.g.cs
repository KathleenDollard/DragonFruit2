using DragonFruit2;
using SampleConsoleApp;

public class Cli
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
    public static Result<MyArgs> ParseArgs<TRootArgs>(string[]? args = null)
        where TRootArgs : MyArgs, IArgs<TRootArgs>
    {
        // TRootArgs is not used, but is retained, along with it's name to reduce confusion at the call site
        return new Builder<MyArgs, MyArgs.MyArgsBuilder>(args).ParseArgs(args);
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
    /// <param name="result">An out parameter that contains an instance of the requested class and supporting data, such as diagnostics, a suggested CLI return value, etc.</param>
    /// <param name="args">Optionaly pass the commandline args</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool TryParseArgs<TRootArgs>(out Result<MyArgs> result, string[]? args = null)
            where TRootArgs : MyArgs, IArgs<TRootArgs>
    {
        result = ParseArgs<TRootArgs>(args);
        return result.IsValid;
    }

}
