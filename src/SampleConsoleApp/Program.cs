using DragonFruit2;
using SampleConsoleApp;

Console.WriteLine("Welcome to the Sample Console App!");
Console.WriteLine();

if (Cli.TryParseArgs<MyArgs>(out var result))
{
    var myArgs = result.Args!; // Safe to use '!' because IsValid is true
    var drink = ", would you like some wine?.";
    var noDrink = ".";
    Console.WriteLine($"{myArgs.Greeting} {myArgs.Name}{(myArgs.Age >= 18 ? drink : noDrink)}");
}
else
{
    result.ReportErrorsToConsole();
}

Console.WriteLine();
Console.WriteLine("Goodbye from the Sample Console App!");
