using DragonFruit2;
using SampleConsoleApp;

var myArgs = Command.ParseArgs<MyArgs>(args);

if (myArgs != null)
{
    var drink = ", would you like some wine?.";
    var noDrink = "c.";
    Console.WriteLine($"{myArgs.Greeting} {myArgs.Name}{(myArgs.Age >= 18 ? drink : noDrink)}");
}
