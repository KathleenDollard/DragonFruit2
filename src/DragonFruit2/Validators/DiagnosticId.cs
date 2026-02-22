namespace DragonFruit2.Validators;

public enum DiagnosticId
{
    // Validators
    GreaterThan = 1,
    GreaterThanOrEqual = 2,
    LessThan = 3,
    LessThanOrEqual = 4,
    Required = 5,
    MinLength = 6,
    MaxLength = 7,

    // System.CommandLine integration
    SystemCommandLine = 100,
    UnknownParsingFailure = 101,
    UnexpectedException = 102,

    // DragonFruit core
    NoActiveCommand = 200,
    // Message for CouldNotFindBuilder should include "Generation may not have run"
    CouldNotFindBuilder = 201,

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