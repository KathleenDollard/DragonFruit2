using DragonFruit2;
using SampleConsoleApp;

Console.WriteLine("Welcome to the Sample Console App!");
Console.WriteLine();

if (Cli.TryParseArgs<MyCommand>(out var result))
{
    var myArgs = result.Command!; // Safe to use '!' because IsValid is true
    var drink = "would you like some wine?.";
    var noDrink = $"would you like some milk, you are {myArgs.Age} years old.";
    Console.WriteLine($"{myArgs.Greeting} {myArgs.Name}, {(myArgs.Age >= 18 ? drink : noDrink)}");
}
else
{
    result.ReportErrorsToConsole();
}

var otherArgsResult = Cli.ParseArgs<MyOtherArgs>();

Console.WriteLine();
Console.WriteLine("Goodbye from the Sample Console App!");
