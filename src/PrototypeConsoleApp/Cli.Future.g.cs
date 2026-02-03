using DragonFruit2;
using SampleConsoleApp;

public class Cli
{
    /// <summary>
    /// Advanced: Creates a Builder, which can be configured, the System.CommandLine API can be accessed, 
    /// and which can be reused (especially helpful in testing). 
    /// </summary>
    /// <remarks>
    /// You may need to build after editing this line.
    /// </remarks>
    /// <typeparam name="TRootArgs">The type containing the CLI definition</typeparam>
    /// <returns>A Result instance containing the hydrated args or error messages.</returns>
    public static Builder<TRootArgs> CreateBuilder<TRootArgs>()
        where TRootArgs : ArgsRootBase<TRootArgs>
    {
        if (typeof(TRootArgs) == typeof(MyArgs))
        {
            var builder = new Builder<MyArgs>(new MyArgs.MyArgsBuilder(null, null));
            return builder is Builder<TRootArgs> typedBuilder 
                ? typedBuilder 
                : throw new InvalidOperationException("Type mismatch creating builder.");
        }
        return null;
        // Add the other root args types here as else if blocks
    }

    /// <summary>
    /// Parses CLI arguments to fill the specified args type. 
    /// </summary>
    /// <remarks>
    /// The args class specified as the type argument must be public.
    /// <br/>
    /// You may need to build after editing this line.
    /// </remarks>
    /// <typeparam name="TRootArgs">The type containing the CLI definition</typeparam>
    /// <param name="args">Optionaly pass the commandline args, if it is not passed, it will be retrieved from System.Environment.</param>
    /// <returns>A Result instance containing the hydrated args or error messages.</returns>
    public static Result<TRootArgs> ParseArgs<TRootArgs>(string[]? args = null)
        where TRootArgs : ArgsRootBase<TRootArgs>
    {
        return CreateBuilder<TRootArgs>().ParseArgs(args);
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
    public static bool TryParseArgs<TRootArgs>(out Result<TRootArgs> result, string[]? args = null)
            where TRootArgs : ArgsRootBase<TRootArgs>
    {
        result = ParseArgs<TRootArgs>(args);
        return result.IsValid;
    }

}
