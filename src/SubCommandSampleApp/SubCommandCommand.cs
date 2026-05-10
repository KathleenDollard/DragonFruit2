using DragonFruit2.Validators;
using DragonFruit2;

namespace SampleConsoleApp;

/// <summary>
/// This is a test command
/// </summary>
[CommandClass]
public partial class SubCommandCommand : CommandClass
{
    /// <summary>
    /// "Your name"
    /// </summary>
    public required string Name { get; set; }
    public string Greeting { get; set; } = "Hello";
}

[CommandClass]
public partial class MorningCommand : SubCommandCommand
{
}

[CommandClass]
public partial class EveningCommand : SubCommandCommand
{

    /// <summary>
    /// "Your age"
    /// </summary>
    [GreaterThan(0)]
    public int Age { get; init; } = 1;
}

[CommandClass]
public partial class Bar : EveningCommand { }