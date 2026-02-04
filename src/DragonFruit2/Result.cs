using DragonFruit2.Validators;
using System.Xml.Schema;

namespace DragonFruit2;

public class Result<TRootArgs>
    where TRootArgs : ArgsRootBase<TRootArgs>
{
    private readonly List<Diagnostic> diagnostics = new();

    public Result(string[] commandLineArguments)
    {
        CommandLineArguments = commandLineArguments;
    }

    public TRootArgs? Args { get; internal set; }
    public IEnumerable<Diagnostic> ValidationFailures => diagnostics;
    public string[] CommandLineArguments { get;  }
    public bool IsValid => !ValidationFailures.Any();
    public CommandDataDefinition<TRootArgs>? ActiveCommandDefinition { get; internal set; }
    public DataValues<TRootArgs>? DataValues { get; internal set; }
    public int SuggestedReturnValue => throw new NotImplementedException();

    public void AddDiagnostic(Diagnostic failure)
        => diagnostics.Add(failure);
    public void AddDiagnostics(IEnumerable<Diagnostic> failures)
     => diagnostics.AddRange(failures);

    public void ReportErrorsToConsole()
    {
        if (ValidationFailures.Any())
        {
            Console.WriteLine("The input was not valid. Problems included:");
            foreach (Diagnostic failure in ValidationFailures)
            {
                Console.WriteLine($"* {failure.Message}");
            }
            Console.WriteLine();
        }
    }
}
