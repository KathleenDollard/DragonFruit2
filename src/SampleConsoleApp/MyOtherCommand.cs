using DragonFruit2;

namespace SampleConsoleApp;

/// <summary>
/// This is a test command
/// </summary>
[CommandClass]
public partial class MyOtherCommand : CommandClass
{
    /// <summary>
    /// "Your name"
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// "Your name"
    /// </summary>
    public required string LastName { get; set; }

}