using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DragonFruit2.GeneratorSupport;

public record class PropInfo
{
    private string? cliName;

    public required string Name { get; init; }
    public string? CliName
    {
        get => cliName switch
        {
            null => Name.ToKebabCase(),
            _ => cliName
        };
        init => cliName = value;
    }
    public required string TypeName { get; init; }
    public bool IsValueType { get; init; }
    public NullableAnnotation NullableAnnotation { get; init; }
    public bool HasRequiredModifier { get; init; }
    public string? Description { get; init; }
    public bool HasArgumentAttribute { get; init; }
    public bool IsArgument { get; init; }

    // If position is not set for all arguments, the argument order is indeterminiate
    public int Position { get; init; } = -1;
    public bool HasInitializer { get; init; }
    public string? InitializerText { get; init; }
    public bool IsRequiredForCli 
        => HasRequiredModifier
                ? true
                : IsValueType
                    ? false
                    : // reference type
                      NullableAnnotation == NullableAnnotation.NotAnnotated && !HasInitializer;

}



