using DragonFruit2.Validators;

namespace DragonFruit2.Test.Validators;

public class ValidatorInfrastructureTests
{
    [Theory]
    [InlineData(DiagnosticId.GreaterThan, "DR001", "Value must be greater than the specified minimum.")]
    [InlineData(DiagnosticId.GreaterThanOrEqual, "DR002", "Value must be greater than or equal to the specified minimum.")]
    [InlineData(DiagnosticId.LessThan, "DR003", "Value must be less than the specified maximum.")]
    [InlineData(DiagnosticId.LessThanOrEqual, "DR004", "Value must be less than or equal to the specified maximum.")]
    [InlineData(DiagnosticId.Required, "DR005", "A value is required for this field.")]
    [InlineData(DiagnosticId.MinLength, "DR006", "The length of the value must be at least the specified minimum.")]
    [InlineData(DiagnosticId.MaxLength, "DR007", "The length of the value must be at most the specified maximum.")]
    [InlineData(DiagnosticId.SystemCommandLine, "DR100", "An error occurred in System.CommandLine while parsing arguments.")]
    [InlineData(DiagnosticId.UnknownParsingFailure, "DR101", "An unknown error occurred while parsing arguments.")]
    [InlineData(DiagnosticId.UnexpectedException, "DR102", "An unexpected exception occurred during processing.")]
    [InlineData(DiagnosticId.NoActiveCommand, "DR200", "No active command could be determined from the provided arguments.")]
    [InlineData(DiagnosticId.CouldNotFindBuilder, "DR201", "Could not find a builder for the root argument type. Generation may not have run.")]
    public void Diagnostic_id_extensions_cover_known_values(DiagnosticId id, string expectedValidationId, string expectedMessage)
    {
        Assert.Equal(expectedValidationId, id.ToValidationIdString());
        Assert.Equal(expectedMessage, id.Message());
        Assert.Equal(expectedValidationId, Validator.ToValidationIdString(id));
    }

    [Fact]
    public void Diagnostic_id_extensions_handle_unknown_values()
    {
        var id = (DiagnosticId)999;

        Assert.Equal("DR999", id.ToValidationIdString());
        Assert.Equal("An unknown validation error occurred.", id.Message());
        Assert.Equal("DR999", Validator.ToValidationIdString(999));
    }

    [Fact]
    public void Diagnostic_records_store_values()
    {
        var diagnostic = new Diagnostic<int>("DR123", DiagnosticSeverity.Warning, "Age", 42, "Problem");

        Assert.Equal("DR123", diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
        Assert.Equal("Age", diagnostic.ValueName);
        Assert.Equal(42, diagnostic.Value);
        Assert.Equal("Problem", diagnostic.Message);
    }

    [Fact]
    public void Validator_attribute_types_store_constructor_values()
    {
        var validatorAttribute = new ValidatorAttributeAttribute(typeof(GreaterThanValidator<>));
        var defaultAttribute = new DefaultAttributeAttribute(typeof(string));
        var greaterThan = new GreaterThanAttribute(5);
        var lessThan = new LessThanAttribute(10);
        var minLength = new MinLengthAttribute(3);
        var maxLength = new MaxLengthAttribute(8);

        Assert.Equal(MemberAttributeAttribute.MemberAttributeKind.Validator, validatorAttribute.Kind);
        Assert.Equal(typeof(GreaterThanValidator<>), validatorAttribute.HelperType);
        Assert.Equal(MemberAttributeAttribute.MemberAttributeKind.Default, defaultAttribute.Kind);
        Assert.Equal(typeof(string), defaultAttribute.HelperType);
        Assert.Equal(5, greaterThan.CompareWith);
        Assert.Equal(10, lessThan.CompareWith);
        Assert.Equal(3, minLength.MinLengthValue);
        Assert.Equal(8, maxLength.MaxLengthValue);
        Assert.IsType<RequiredAttribute>(new RequiredAttribute());
        Assert.IsAssignableFrom<ValidatorBaseAttribute>(new TestValidatorBaseAttribute());
        Assert.IsType<DefaultBaseAttribute>(new DefaultBaseAttribute());
    }

    private sealed class TestValidatorBaseAttribute : ValidatorBaseAttribute
    {
    }
}
