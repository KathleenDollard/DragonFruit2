using DragonFruit2.Validators;

namespace DragonFruit2.Test.Validators;

public class LessThanValidatorTests
{
    [Fact]
    public void ValidateLessThan_extension_creates_validator()
    {
        var name = "Age";
        var compareWith = 11;
        var member = new OptionDataDefinition<int>(null!, name)
        {
            IsRequired = true,
            DataType = typeof(int)
        };

        member.ValidateLessThan(compareWith);

        Assert.Single(member.Validators);
        var validator = Assert.IsType<LessThanValidator<int>>(member.Validators.Single());
        Assert.Equal(name, validator.ValueName);
        Assert.Equal(compareWith, validator.CompareWithValue);
    }

    [Theory]
    [InlineData(1, 2, true)]
    [InlineData(2, 2, false)]
    [InlineData(3, 2, false)]
    [InlineData(-1, 0, true)]
    public void Validate_ints_handled_correctly(int value, int compareWith, bool success)
    {
        var validator = new LessThanValidator<int>("Age", compareWith);
        var dataValue = DataValue<int>.Create("Age", typeof(int), null!, null!);
        dataValue.SetValue(value, StubDataProvider.Instance);

        var diagnostics = validator.Validate(dataValue);

        Assert.NotEqual(success, diagnostics.Any());
    }

    [Fact]
    public void Validate_reference_type_allows_null_without_diagnostic()
    {
        var validator = new LessThanValidator<string>("Name", "m");
        var dataValue = DataValue<string>.Create("Name", typeof(string), null!, null!);
        dataValue.SetValue(null!, StubDataProvider.Instance);

        var diagnostics = validator.Validate(dataValue);

        Assert.Empty(diagnostics);
    }

    [Fact]
    public void Description_and_message_match_less_than_semantics()
    {
        var validator = new LessThanValidator<int>("Age", 5);
        var dataValue = DataValue<int>.Create("Age", typeof(int), null!, null!);
        dataValue.SetValue(6, StubDataProvider.Instance);

        var diagnostic = Assert.Single(validator.Validate(dataValue));

        Assert.Equal("The value of Age must be less than 5", validator.Description);
        Assert.Equal("The value of Age must be less than 5, and 6 is not.", diagnostic.Message);
    }
}
