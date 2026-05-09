using DragonFruit2;
using SampleConsoleApp;

Console.WriteLine("Welcome to the Sample Console App!");
Console.WriteLine();

if (Cli.TryParseAxgs<MyCommand>(out var result))
{
    var myCommand = result.Command!; // Safe to use '!' because IsValid is true
    var drink = "would you like some wine?.";
    var noDrink = $"would you like some milk, you are {myCommand.Age} years old.";
    Console.WriteLine($"{myCommand.Greeting} {myCommand.Name}, {(myCommand.Age >= 18 ? drink : noDrink)}");
}
else
{
    result.ReportErrorsToConsole();
}

var otherCommandResult = Cli.ParseAxgs<MyOtherCommand>();

Console.WriteLine();
Console.WriteLine("Goodbye from the Sample Console App!");
