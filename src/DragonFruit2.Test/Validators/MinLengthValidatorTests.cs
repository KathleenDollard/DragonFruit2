using DragonFruit2.Validators;

namespace DragonFruit2.Test.Validators;

public class MinLengthValidatorTests
{
    [Fact]
    public void ValidateMinimumLength_extension_creates_validator()
    {
        var name = "Name";
        var minLength = 5;
        var member = new OptionDataDefinition<string>(null!, name)
        {
            IsRequired = true,
            DataType = typeof(string)
        };

        member.ValidateMinimumLength(minLength);

        Assert.Single(member.Validators);
        var validator = Assert.IsType<MinLengthValidator>(member.Validators.Single());
        Assert.Equal(name, validator.ValueName);
        Assert.Equal(minLength, validator.MinLengthValue);
    }

    [Fact]
    public void Constructor_sets_min_length_value()
    {
        var validator = new MinLengthValidator("Name", 5);

        Assert.Equal(5, validator.MinLengthValue);
    }

    [Theory]
    [InlineData("abc", 5, false)]
    [InlineData("abcde", 5, true)]
    [InlineData("abcdef", 5, true)]
    public void Validate_strings_handled_correctly(string value, int minLength, bool success)
    {
        var validator = new MinLengthValidator("Name", minLength);
        var dataValue = DataValue<string>.Create("Name", typeof(string), null!, null!);
        dataValue.SetValue(value, StubDataProvider.Instance);

        var diagnostics = validator.Validate(dataValue);

        Assert.NotEqual(success, diagnostics.Any());
    }

    [Fact]
    public void Description_and_message_match_min_length_semantics()
    {
        var validator = new MinLengthValidator("Name", 5);
        var dataValue = DataValue<string>.Create("Name", typeof(string), null!, null!);
        dataValue.SetValue("abc", StubDataProvider.Instance);

        var diagnostic = Assert.Single(validator.Validate(dataValue));

        Assert.Equal("The string Name must be longer than 5", validator.Description);
        Assert.Equal("The value of Name must be greater than 5, and abc is not.", diagnostic.Message);
    }
}
