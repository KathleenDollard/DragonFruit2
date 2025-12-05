namespace ExerciseDragonFruit2;

public partial class MyArgs2
{
    /// <summary>
    /// "Your name"
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// "Your age"
    /// </summary>
    public int Age { get; set; } = 0;

    /// <summary>
    /// "Greeting message"
    /// </summary>
    public string Greeting { get; set; } = string.Empty;

    /// <summary>
    /// "Question text"
    /// </summary>
    public string Over18Question { get; set; } = "Would you like some wine?";

}
