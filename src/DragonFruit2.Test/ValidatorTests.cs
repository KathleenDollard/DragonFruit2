using DragonFruit2.Validators;

namespace DragonFruit2.Test;

public class ValidatorTests
{
    [Fact]
    public void GreaterThanValidator_extension_creates_validator()
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
        var validator = member.Validators.OfType<GreaterThanValidator<int>>(). Single();
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
    public void GreaterThanValidator_ints_handled_correctly(int value, int compareWith, bool success)
    {
        var validator = new GreaterThanValidator<int>("Age", compareWith);
        var dataValue = DataValue<int>.Create("Age", typeof(int), null!, null!);
        dataValue.SetValue(value, StubDataProvider.Instance);

        var diagnostics = validator.Validate(dataValue);

        Assert.NotEqual(success, diagnostics.Any());
    }
}
