using DragonFruit2.Validators;

namespace DragonFruit2.Test;

public class DefaultTests
{
    [Fact]
    public void DefaultConstant_extension_creates_validator()
    {
        var name = "Age";
        var value = 42;
        var member = new OptionDataDefinition<int>(null!, name)
        {
            IsRequired = true,
            DataType = typeof(int)
        };

        // This should go to the DataProvider
        member.Default(value);


    }

    [Theory]
    [InlineData(1, 0, true)]
    [InlineData(-1, 0, false)]
    [InlineData(0, 0, false)]
    [InlineData(43, 42, true)]
    [InlineData(41, 42, false)]
    [InlineData(42, 42, false)]
    public void DefaultConst_handled_correctly(int value, int compareWith, bool success)
    {
        var validator = new GreaterThanValidator<int>("Age", compareWith);
        var dataValue = DataValue<int>.Create("Age", typeof(int), null!, null!);
        dataValue.SetValue(value, StubDataProvider.Instance);

        var diagnostics = validator.Validate(dataValue);

        Assert.NotEqual(success, diagnostics.Any());
    }
}
