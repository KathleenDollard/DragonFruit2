namespace DragonFruit2;

public readonly record struct DataValue<T>
{
    public static DataValue<T> Create(T value, DataProvider setBy)
        => new(value, setBy);

    public static DataValue<T> CreateEmpty()
        => new();

    private DataValue(T value, DataProvider setBy)
    {
        Value = value;
        SetBy = setBy;
    }

    public T? Value { get; }

    public DataProvider? SetBy { get; }

    public bool IsSet => SetBy is not null;
}
