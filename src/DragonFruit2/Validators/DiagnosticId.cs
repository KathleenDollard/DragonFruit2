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

        public string Message()
        {
            return id switch
            {
                DiagnosticId.GreaterThan => "Value must be greater than the specified minimum.",
                DiagnosticId.GreaterThanOrEqual => "Value must be greater than or equal to the specified minimum.",
                DiagnosticId.LessThan => "Value must be less than the specified maximum.",
                DiagnosticId.LessThanOrEqual => "Value must be less than or equal to the specified maximum.",
                DiagnosticId.Required => "A value is required for this field.",
                DiagnosticId.MinLength => "The length of the value must be at least the specified minimum.",
                DiagnosticId.MaxLength => "The length of the value must be at most the specified maximum.",
                DiagnosticId.SystemCommandLine => "An error occurred in System.CommandLine while parsing arguments.",
                DiagnosticId.UnknownParsingFailure => "An unknown error occurred while parsing arguments.",
                DiagnosticId.UnexpectedException => "An unexpected exception occurred during processing.",
                DiagnosticId.NoActiveCommand => "No active command could be determined from the provided arguments.",
                DiagnosticId.CouldNotFindBuilder => "Could not find a builder for the root argument type. Generation may not have run.",
                _ => "An unknown validation error occurred."
            };
        }
    }
}