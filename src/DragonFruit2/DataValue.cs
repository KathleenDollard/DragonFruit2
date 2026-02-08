namespace DragonFruit2;

public abstract record class DataValue
{
    public abstract bool Validate();
    protected abstract IEnumerable<Diagnostic>? UntypedDiagnostics { get; }
    public IEnumerable<Diagnostic>? Diagnostics => UntypedDiagnostics;

    public DataValue(string name, Type argsType, DataValues dataValues, MemberDataDefinition memberDefinition)
    {
        MemberDefinition = memberDefinition;
        Name = name;
        DataValues = dataValues;
        ArgsType = argsType;
    }

    public string Name { get; }
    public DataValues DataValues { get; }
    public Type ArgsType { get; }
    public DataProvider? SetBy { get; protected set; }
    public bool IsSet => SetBy is not null; public MemberDataDefinition MemberDefinition { get; }

}

public record class DataValue<TValue> : DataValue
{
    public static DataValue<TValue> Create(string name, Type argsType, DataValues dataValues, MemberDataDefinition<TValue> memberDefinition)
        => new(name, argsType, dataValues, memberDefinition);

    private DataValue(string name, Type argsType, DataValues dataValues, MemberDataDefinition<TValue> memberDefinition)
        : base(name, argsType, dataValues, memberDefinition)
    {
        MemberDefinition = memberDefinition;
    }

    public new MemberDataDefinition<TValue> MemberDefinition { get; }
    public TValue? Value { get; private set; }
    private List<Diagnostic<TValue>>? _diagnostics;
    public new IEnumerable<Diagnostic<TValue>>? Diagnostics => _diagnostics;

    protected override IEnumerable<Diagnostic>? UntypedDiagnostics 
        => Diagnostics?.OfType<Diagnostic>();

    public void SetValue(TValue value, DataProvider setBy)
    {
        SetBy = setBy;
        Value = value;
    }

    private void AddDiagnostic(Diagnostic diagnostic)
    {
        if (diagnostic is not Diagnostic<TValue> typedDiagnostic)
        {
            throw new InvalidOperationException("Unexpected diagnostic type");
        }
        _diagnostics ??= [];
        _diagnostics.Add(typedDiagnostic);
    }

    private void AddDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            AddDiagnostic(diagnostic);
        }
    }

    public override bool Validate()
    {
        var newDiagnostics = MemberDefinition.Validate(this);
        if (newDiagnostics is null || !newDiagnostics.Any())
        {  // No validation issues found
            return true;
        }
        AddDiagnostics(newDiagnostics); 
        return false;
    }

}
