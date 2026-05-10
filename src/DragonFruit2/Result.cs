namespace DragonFruit2;

public abstract class Result
{
    public abstract IEnumerable<Diagnostic> Diagnostics { get; }
    private readonly List<Diagnostic> diagnostics = [];
    // For .NET Framework runtimes, it is important not to recreate the Random instance as the seed can be repeated in a loop, such as when we support scripts
    private static readonly Random random = new();

    public Result(string[] commandLineArguments)
    {
        CommandLineArguments = commandLineArguments;
        RunId = random.Next();
    }

    public int RunId { get; init; }
    public DragonFruit2Configuration Configuration { get; } = new();
    protected IEnumerable<Diagnostic> CommandDiagnostics => diagnostics;

    public string[] CommandLineArguments { get; }
    public bool IsValid => !Diagnostics.Any();
    public int SuggestedReturnValue => throw new NotImplementedException();

    public void AddDiagnostic(Diagnostic failure)
        => diagnostics.Add(failure);
    public void AddDiagnostics(IEnumerable<Diagnostic> failures)
     => diagnostics.AddRange(failures);

    public void ReportErrorsToConsole()
    {
        if (Diagnostics.Any())
        {
            Console.WriteLine("The input was not valid. Problems included:");
            foreach (Diagnostic failure in Diagnostics)
            {
                var message = failure.Message is null
                    ? failure.Id
                    : failure.Message;
                Console.WriteLine($"* {message}");
            }
            Console.WriteLine();
        }
    }

}

public class Result<TRootCommand> : Result
{

    public Result(string[] commandLineArguments) : base(commandLineArguments)
    {
    }

    public TRootCommand? Command { get; internal set; }
    public DataValues<TRootCommand>? DataValues { get; internal set; }

    public CommandDataDefinition<TRootCommand>? ActiveCommandDefinition

    {
        get;
        internal set
        {
            if (value is null)
            { throw new ArgumentNullException(nameof(value)); }
            field = value;
            DataValues = field.CreateDataValues();
        }
    }

    public DataProvider<TRootCommand> ActiveDataProvider { get; internal set; }

    public override IEnumerable<Diagnostic> Diagnostics 
        => DataValues switch
        {
            null => CommandDiagnostics,
            _ when DataValues.All(d => d.Diagnostics is null || !d.Diagnostics.Any()) => CommandDiagnostics,
            _ => CommandDiagnostics
                    .Concat(DataValues
                        .Where(d => d.Diagnostics is not null && d.Diagnostics.Any())
                        .SelectMany(d => d.Diagnostics))
        };


    public void Cleanup()
    {

        if (!Configuration.ResultDebuggingLevel.HasFlag(ResultDebuggingLevel.DataValues))
        {
            DataValues = null;
        }

    }
}

public class Result<TCommand, TRootCommand> : Result<TRootCommand>
{

    public Result(string[] commandLineArguments) : base(commandLineArguments)
    {
    }
}
