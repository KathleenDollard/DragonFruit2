using DragonFruit2;
using DragonFruit2.Validators;

namespace SampleConsoleApp;

/// <summary>
/// This is a test command
/// </summary>
public partial class MyArgs : ArgsRootBase<MyArgs>
{
    /// <summary>
    /// "Your name"
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// "Your age"
    /// </summary>
    [GreaterThan(0)]
    public int Age { get; init; } = 1;

    /// <summary>
    /// "Greeting message"
    /// </summary>
    public string Greeting { get; init; } = string.Empty;


}