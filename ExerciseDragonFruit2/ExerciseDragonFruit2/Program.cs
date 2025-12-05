using DragonFruit2;
using ExerciseDragonFruit2;

var myArgs = Cli.ParseArgs<MyArgs2>(args);

if (myArgs is not null)
{
    var noDrink = ".";
    Console.WriteLine($"{myArgs.Greeting} {myArgs.Name}{(myArgs.Age >= 18 ? ", " + myArgs.Over18Question : noDrink)}");
}