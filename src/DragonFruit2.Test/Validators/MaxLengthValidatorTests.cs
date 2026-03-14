using DragonFruit2.Validators;

namespace DragonFruit2.Test.Validators;

public class MaxLengthValidatorTests
{
    [Fact]
    public void ValidateMaxLength_extension_creates_validator()
    {
        var name = "Name";
        var maxLength = 5;
        var member = new OptionDataDefinition<string>(null!, name)
        {
            IsRequired = true,
            DataType = typeof(string)
        };

        member.ValidateMaxLength(maxLength);

        Assert.Single(member.Validators);
        var validator = Assert.IsType<MaxLengthValidator>(member.Validators.Single());
        Assert.Equal(name, validator.ValueName);
        Assert.Equal(maxLength, validator.MaxLengthValue);
    }

    [Fact]
    public void Constructor_sets_max_length_value()
    {
        var validator = new MaxLengthValidator("Name", 5);

        Assert.Equal(5, validator.MaxLengthValue);
    }

    [Theory]
    [InlineData("abc", 5, true)]
    [InlineData("abcde", 5, true)]
    [InlineData("abcdef", 5, false)]
    public void Validate_strings_handled_correctly(string value, int maxLength, bool success)
    {
        var validator = new MaxLengthValidator("Name", maxLength);
        var dataValue = DataValue<string>.Create("Name", typeof(string), null!, null!);
        dataValue.SetValue(value, StubDataProvider.Instance);

        var diagnostics = validator.Validate(dataValue);

        Assert.NotEqual(success, diagnostics.Any());
    }

    [Fact]
    public void Description_and_message_match_max_length_semantics()
    {
        var validator = new MaxLengthValidator("Name", 5);
        var dataValue = DataValue<string>.Create("Name", typeof(string), null!, null!);
        dataValue.SetValue("abcdef", StubDataProvider.Instance);

        var diagnostic = Assert.Single(validator.Validate(dataValue));

        Assert.Equal("The string Name must not be longer than 5", validator.Description);
        Assert.Equal("The value of Name must not be longer than 5, and abcdef is.", diagnostic.Message);
    }
}
