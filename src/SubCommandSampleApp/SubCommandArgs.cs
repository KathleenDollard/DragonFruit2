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
    public string Greeting { get; set; } = "Hello";
}

public partial class MorningArgs : SubCommandArgs
{
}

public partial class EveningArgs : SubCommandArgs
{

    /// <summary>
    /// "Your age"
    /// </summary>
    [GreaterThan(0)]
    public int Age { get; init; } = 1;
}

public partial class Bar : EveningArgs { }