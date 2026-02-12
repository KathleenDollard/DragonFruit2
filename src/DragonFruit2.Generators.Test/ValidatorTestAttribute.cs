namespace DragonFruit2.Generators.Test;

public class ValidatorTest { }

public sealed class ValidatorTestAttributeOneCtorParam : ValidatorAttribute
{

    // This is a positional argument
    public ValidatorTestAttributeOneCtorParam(object compareWith)
        : base("ValidatorTest")
    {
        CompareWith = compareWith;
    }

    public object CompareWith { get; }


    public int AnotherValue { get; set; }
}

public sealed class ValidatorTestAttributeOneNamedParam : ValidatorAttribute
{
    public ValidatorTestAttributeOneNamedParam()
        : base("ValidatorTest")
    {}


    public int AnotherValue { get; set; }
}



