using DragonFruit2.Validators;
using System.Xml.Schema;

namespace DragonFruit2;

public class Result<TArgs>
    where TArgs : ArgsRootBase<TArgs>
{
    private readonly List<ValidationFailure> validationFailures = new();

    public Result(IEnumerable<ValidationFailure>? failures, TArgs? args)
    {
        validationFailures.AddRange(failures);
        Args = args;
        Status = (failures, args) switch
        {
            (not null, null) when failures.Any(x => x.Severity == DiagnosticSeverity.Error) => ResultStatus.Invalid,
            (_, null) => ResultStatus.SclHandled,
            (_, not null) => ResultStatus.ReadyToRun,
        }; ;
    }

    public IEnumerable<ValidationFailure> ValidationFailures => validationFailures;
    public TArgs? Args { get; set; }
    public ResultStatus Status { get; internal set; }
    public bool IsValid => Status == ResultStatus.ReadyToRun;
    public int SuggestedReturnValue => throw new NotImplementedException();

    public void AddFailure(ValidationFailure failure)
        => validationFailures.Add(failure);
    public void AddFailures(IEnumerable<ValidationFailure> failures)
     => validationFailures.AddRange(failures);

    public void ReportErrorsToConsole()
    {
        if (ValidationFailures.Any())
        {
            Console.WriteLine("The input was not valid. Problems included:");
            foreach (ValidationFailure failure in ValidationFailures)
            {
                Console.WriteLine($"* {failure.Message}");
            }
            Console.WriteLine();
        }
    }
}
