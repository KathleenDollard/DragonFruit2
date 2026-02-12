
using System.Globalization;

namespace DragonFruit2.Generators;

public class ValidatorInfo
{
    public required string AttributeName { get; init; }
    public required string ValidatorName { get; init;  }
    public required ValidatorArgumentInfo[] ValidatorValues { get; init;}

}

public class ValidatorArgumentInfo
{
    public required string Name { get; init; }
    public required string ValidatorTypeName { get; init; }
    public required string ArgumentTypeName { get; init; }
    public required string Value {  get; init; }
}
