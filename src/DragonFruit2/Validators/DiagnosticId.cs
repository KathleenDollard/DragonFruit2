namespace DragonFruit2.Validators;

public enum DiagnosticId
{
    // Validators
    GreaterThan = 1,
    GreaterThanOrEqual = 2, 
    LessThan = 3,
    LessThanOrEqual = 4,
    Required = 5,

    // System.CommandLine integration
    SystemCommandLine = 100,
    UnknownParsingFailure = 101,
    MinLength = 102,
    MaxLength = 103,

    // DragonFruit core
    NoActiveCommand = 200,

}

public static class ValidationIdExtensions
{
    private static readonly string DragonFruitValidationPrefix = "DR";

    extension(DiagnosticId id)
    {
        public string ToValidationIdString()
        {
            return $"{DragonFruitValidationPrefix}{(int)id:000}";
        }
    }
}