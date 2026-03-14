using DragonFruit2.Validators;

namespace DragonFruit2.Test.Validators;

public class GreaterThanValidatorTests
{
    [Fact]
    public void ValidateGreaterThan_extension_creates_validator()
    {
        var name = "Age";
        var compareWith = 11;
        var member = new OptionDataDefinition<int>(null!, name)
        {
            IsRequired = true,
            DataType = typeof(int)
        };

        member.ValidateGreaterThan(compareWith);

        Assert.Single(member.Validators);
        Assert.IsType<GreaterThanValidator<int>>(member.Validators.Single());
        var validator = member.Validators.OfType<GreaterThanValidator<int>>().Single();
        Assert.Equal(name, validator.ValueName);
        Assert.Equal(compareWith, validator.CompareWithValue);
    }

    [Theory]
    [InlineData(1, 0, true)]
    [InlineData(-1, 0, false)]
    [InlineData(0, 0, false)]
    [InlineData(43, 42, true)]
    [InlineData(41, 42, false)]
    [InlineData(42, 42, false)]
    public void Validate_ints_handled_correctly(int value, int compareWith, bool success)
    {
        var validator = new GreaterThanValidator<int>("Age", compareWith);
        var dataValue = DataValue<int>.Create("Age", typeof(int), null!, null!);
        dataValue.SetValue(value, StubDataProvider.Instance);

        var diagnostics = validator.Validate(dataValue);

        Assert.NotEqual(success, diagnostics.Any());
    }

    [Fact]
    public void Validate_reference_type_allows_null_without_diagnostic()
    {
        var validator = new GreaterThanValidator<string>("Name", "m");
        var dataValue = DataValue<string>.Create("Name", typeof(string), null!, null!);
        dataValue.SetValue(null!, StubDataProvider.Instance);

        var diagnostics = validator.Validate(dataValue);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public void Description_and_message_match_greater_than_semantics()
    {
        var validator = new GreaterThanValidator<int>("Age", 5);
        var dataValue = DataValue<int>.Create("Age", typeof(int), null!, null!);
        dataValue.SetValue(4, StubDataProvider.Instance);

        var diagnostic = Assert.Single(validator.Validate(dataValue));

        Assert.Equal("The value of Age must be greater than 5", validator.Description);
        Assert.Equal("The value of Age must be greater than 5, and 4 is not.", diagnostic.Message);
        Assert.Equal("DR001", validator.Id);
    }
}
