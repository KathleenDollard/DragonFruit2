namespace DragonFruit2;

public abstract class Result
{
    public abstract IEnumerable<Diagnostic> Diagnostics { get; }
    private readonly List<Diagnostic> diagnostics = [];

    public Result(string[] commandLineArguments)
    {
        CommandLineArguments = commandLineArguments;
    }

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
                Console.WriteLine($"* {failure.Message}");
            }
            Console.WriteLine();
        }
    }

}

public class Result<TRootArgs> : Result
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    public Result(string[] commandLineArguments) : base(commandLineArguments)
    {
    }

    public TRootArgs? Args { get; internal set; }

    public CommandDataDefinition<TRootArgs>? ActiveCommandDefinition
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
    public DataValues<TRootArgs>? DataValues { get; internal set; }

    public override IEnumerable<Diagnostic> Diagnostics
    {
        get
        {
            // TODO: Check if we need the null check in the following LINQ.
            var memberDiagnostics = DataValues
                                        .Where(d=>d.Diagnostics is not null && d.Diagnostics.Any())
                                        .SelectMany(d=> d.Diagnostics);
            return CommandDiagnostics.Concat(memberDiagnostics);
        }
    }
}
