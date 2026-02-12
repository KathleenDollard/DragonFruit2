
namespace DragonFruit2.Generators;

public class ValidatorInfo
{
    public required string AttributeName { get; init; }
    public required string ValidatorName { get; init;  }
    public required KeyValuePair<string, string>[] ValidatorValues { get; init;}

}
