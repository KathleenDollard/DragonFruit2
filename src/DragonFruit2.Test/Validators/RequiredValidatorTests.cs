using DragonFruit2.Validators;

namespace DragonFruit2.Test.Validators;

public class RequiredValidatorTests
{
    [Fact]
    public void Required_extension_creates_validator()
    {
        var name = "Name";
        var member = new OptionDataDefinition<string>(null!, name)
        {
            IsRequired = true,
            DataType = typeof(string)
        };

        member.Required();

        Assert.Single(member.Validators);
        var validator = Assert.IsType<RequiredValidator<string>>(member.Validators.Single());
        Assert.Equal(name, validator.ValueName);
    }

    [Fact]
    public void Validate_returns_no_diagnostics_when_value_is_set()
    {
        var validator = new RequiredValidator<string>("Name");
        var dataValue = DataValue<string>.Create("Name", typeof(string), null!, null!);
        dataValue.SetValue("value", StubDataProvider.Instance);

        var diagnostics = validator.Validate(dataValue);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public void Validate_returns_diagnostic_when_value_is_not_set()
    {
        var validator = new RequiredValidator<string>("Name");
        var dataValue = DataValue<string>.Create("Name", typeof(string), null!, null!);

        var diagnostic = Assert.Single(validator.Validate(dataValue));

        Assert.Equal("The value of Name is required", validator.Description);
        Assert.Equal("The value of Name is required and it was not entered.", diagnostic.Message);
        Assert.Equal(DiagnosticId.Required.ToValidationIdString(), diagnostic.Id);
    }
}
