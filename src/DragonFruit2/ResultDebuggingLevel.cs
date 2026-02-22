namespace DragonFruit2;

[Flags]
public enum ResultDebuggingLevel
{
    None = 0,
    DataValues = 1,
    // DataProviders = 2, // Future enhancement
    All = int.MaxValue,
}
