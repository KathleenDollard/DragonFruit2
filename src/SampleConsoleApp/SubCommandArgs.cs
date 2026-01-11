using DragonFruit2.Validators;

namespace SampleConsoleApp;

/// <summary>
/// This is a test command
/// </summary>
public partial class SubCommandArgs
{
    /// <summary>
    /// "Your name"
    /// </summary>
    public required string Name { get; set; }
}

public partial class MorningGreetingArgs : SubCommandArgs
{
}

public partial class EveningGreetingArgs : SubCommandArgs
{

    /// <summary>
    /// "Your age"
    /// </summary>
    [GreaterThan(0)]
    public int Age { get; init; } = 1;
}