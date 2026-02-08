namespace DragonFruit2.Validators;

/// <summary>
/// Provides a base class for implementing value validators with a unique identifier and descriptive metadata.
/// </summary>
/// <remarks>
/// The non-generic class makes it easier to supply help information on validation.
/// </remarks>
public abstract class Validator
{
    public static string ToValidationIdString(int id) => $"DR{id:000}";

    public static string ToValidationIdString(DiagnosticId id) => ToValidationIdString((int)id);

    protected Validator(int id, string valueName)
    {
        IdValue = id;
        ValueName = valueName;
    }

    public string Id => ToValidationIdString(IdValue);
    public int IdValue { get; }
    public string ValueName { get; }
    public abstract string Description { get; }
}

public abstract class Validator<TValue> : Validator
{
    protected Validator(int id, string valueName)
        : base(id, valueName)
    {  }

    public abstract IEnumerable<Diagnostic<TValue>> Validate( DataValue<TValue> value);
}
