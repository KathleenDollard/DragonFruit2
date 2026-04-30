//using DragonFruit2.Common;
using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators.Metadata;

public record class PropInfo
{
    private string? cliName;

    public required string Name { get; init; }
    public string? CliName
    {
        get => cliName switch
        {
            null when IsArgument=> Name.ToUpper(),
            null => $"--{Name.ToKebabCase()}",
            _ => cliName
        };
        init => cliName = value;
    }
    public required string TypeName { get; init; }
    public required string ContainingTypeName { get; init; }
    public bool IsValueType { get; init; }
    public NullableAnnotation NullableAnnotation { get; init; }
    public bool HasRequiredModifier { get; init; }
    public string? Description { get; init; }
    public bool HasArgumentAttribute { get; init; }
    public bool IsArgument { get; init; }

    // If position is not set for all arguments, the argument order is indeterminiate
    public int Position { get; init; }
    public bool HasInitializer { get; init; }
    public string? InitializerText { get; init; }
    public bool IsRequiredForCli 
        => HasRequiredModifier
                ? true
                : IsValueType
                    ? false
                    : // reference type
                      NullableAnnotation == NullableAnnotation.NotAnnotated && !HasInitializer;

    public List<ValidatorInfo> Validators { get; init; } = [];
    public List<DefaultInfo> Defaults { get; init; } = [];

    public virtual bool Equals(PropInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // Compare the properties of PropInfo
        return Name == other.Name
            && CliName == other.CliName
            && TypeName == other.TypeName
            && ContainingTypeName == other.ContainingTypeName
            && IsValueType == other.IsValueType
            && NullableAnnotation == other.NullableAnnotation
            && HasRequiredModifier == other.HasRequiredModifier
            && Description == other.Description
            && HasArgumentAttribute == other.HasArgumentAttribute
            && IsArgument == other.IsArgument
            && Position == other.Position
            && HasInitializer == other.HasInitializer
            && InitializerText == other.InitializerText
            && Validators.SequenceEqual(other.Validators)
            && Defaults.SequenceEqual(other.Defaults);
    }

    public override int GetHashCode()
    {
        var hashCode = new System.HashCode();
        hashCode.Add(Name);
        hashCode.Add(CliName);
        hashCode.Add(TypeName);
        hashCode.Add(ContainingTypeName);
        hashCode.Add(IsValueType);
        hashCode.Add(NullableAnnotation);
        hashCode.Add(HasRequiredModifier);
        hashCode.Add(Description);
        hashCode.Add(HasArgumentAttribute);
        hashCode.Add(IsArgument);
        hashCode.Add(Position);
        hashCode.Add(HasInitializer);
        hashCode.Add(InitializerText);
        foreach (var validator in Validators)
        {
            hashCode.Add(validator);
        }
        foreach (var defaultInfo in Defaults)
        {
            hashCode.Add(defaultInfo);
        }
        return hashCode.ToHashCode();
    }
}



