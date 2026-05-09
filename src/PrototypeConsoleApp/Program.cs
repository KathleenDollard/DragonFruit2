using SampleConsoleApp;
using DragonFruit2;

var myCommandDataValues = Cli.ParseAxgs<MyCommand>(args);
Console.WriteLine("Welcome to the Sample Console App!") ;

if (myCommandDataValues.IsValid)
{
    var myCommand = myCommandDataValues.Command!; // Safe to use '!' because IsValid is true
    var drink = $", so would you like some wine?.";
    var noDrink = ".";
    Console.WriteLine($"{myCommand.Greeting} {myCommand.Name}, you're {myCommand.Age}{(myCommand.Age >= 18 ? drink : noDrink)}");
}
else
{
    myCommandDataValues.ReportErrorsToConsole();
}

Console.WriteLine("Goodbye from the Sample Console App!");

