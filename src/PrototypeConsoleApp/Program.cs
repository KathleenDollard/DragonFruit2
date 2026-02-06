using SampleConsoleApp;

var myArgsDataValues = Cli.ParseArgs<MyArgs>(args);
Console.WriteLine("Welcome to the Sample Console App!") ;

if (myArgsDataValues.IsValid)
{
    var myArgs = myArgsDataValues.Args!; // Safe to use '!' because IsValid is true
    var drink = ", would you like some wine?.";
    var noDrink = ".";
    Console.WriteLine($"{myArgs.Greeting} {myArgs.Name}{(myArgs.Age >= 18 ? drink : noDrink)}");
}
else
{
    myArgsDataValues.ReportErrorsToConsole();
}

Console.WriteLine("Goodbye from the Sample Console App!");

