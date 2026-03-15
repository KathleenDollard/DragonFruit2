using DragonFruit2.Validators;

namespace DragonFruit2.Test;

public class DiagnosticIdTests
{
    [Fact]
    public void DiagnosticId_values_are_unique()
    {
        // Build "value -> enum names" groups and keep only values assigned to more than one enum member.
        var duplicateGroups = Enum.GetNames(typeof(DiagnosticId))
            .Select(name => (Name: name, Value: (int)Enum.Parse<DiagnosticId>(name)))
            .GroupBy(item => item.Value)
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key}: {string.Join(", ", group.Select(item => item.Name))}")
            .ToArray();

        Assert.True(
            duplicateGroups.Length == 0,
            $"Duplicate DiagnosticId values found: {string.Join("; ", duplicateGroups)}");
    }

    [Fact]
    public void DiagnosticId_values_are_positive_and_less_than_1000()
    {
        // Project each enum name/value pair, then keep only ids outside the supported range (> 0 and < 1000).
        var invalidIds = Enum.GetNames(typeof(DiagnosticId))
            .Select(name => (Name: name, Value: (int)Enum.Parse<DiagnosticId>(name)))
            .Where(item => item.Value <= 0 || item.Value >= 1000)
            .Select(item => $"{item.Name}={item.Value}")
            .ToArray();

        Assert.True(
            invalidIds.Length == 0,
            $"DiagnosticId values must be > 0 and < 1000. Invalid ids: {string.Join(", ", invalidIds)}");
    }

    [Fact]
    public void DiagnosticId_values_have_custom_messages()
    {
        // message returned in the discard pattern, _ => "..."
        const string defaultMessage = "An unknown validation error occurred.";

        var idsUsingDefaultMessage = Enum.GetValues(typeof(DiagnosticId))
            .Cast<DiagnosticId>()
            .Where(id => string.Equals(id.Message(), defaultMessage, StringComparison.Ordinal))
            .Select(id => $"{id}={(int)id}")
            .ToArray();

        Assert.True(
            idsUsingDefaultMessage.Length == 0,
            $"Each DiagnosticId must have a custom message mapping. Missing mappings: {string.Join(", ", idsUsingDefaultMessage)}");
    }
}
