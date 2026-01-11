using DragonFruit2;
using SampleConsoleApp;

#region Simple CLI
var myArgsDataValues = Cli.ParseArgs<MyArgs>(args);
Console.WriteLine("Welcome to the Sample Console App!");
Console.WriteLine();

if (myArgsDataValues.IsValid)
{
    var myArgs = myArgsDataValues.Args!; // Safe to use '!' because IsValid is true
    var drink = ", would you like some wine?.";
    var noDrink = ".";
    Console.WriteLine($"{myArgs.Greeting} {myArgs.Name}{(myArgs.Age >= 18 ? drink : noDrink)}");
}
else
{
    foreach (var failure in myArgsDataValues.ValidationFailures)
    {
        Console.WriteLine($"Error: {failure.Message}");
    }
}
Console.WriteLine();
#endregion

//#region SubCommand example
//var subcommandArgsDataValues = Cli.ParseArgs<SubCommandArgs>(args);
//Console.WriteLine();

//if (subcommandArgsDataValues.IsValid)
//{
//    return subcommandArgsDataValues.Args switch
//    {
//        MorningGreetingArgs morningArgs => MorningGreeting(morningArgs),

//        EveningGreetingArgs eveningArgs => EveningGreetingArgs(eveningArgs),

//        _ => UnknownGreeting()
//    }
//;
//}
//else
//{
//    foreach (var failure in myArgsDataValues.ValidationFailures)
//    {
//        Console.WriteLine($"Error: {failure.Message}");
//    }
//}

//int UnknownGreeting()
//{
//    Console.WriteLine("What the heck?");
//    return 1;
//}

//Console.WriteLine();

//static int MorningGreeting(MorningGreetingArgs morningArgs)
//{
//    var breakfast = ", would you like some Cheerios with chocolate milk?.";
//    Console.WriteLine($"{morningArgs.Greeting} {morningArgs.Name}{breakfast}");
//    return 0;
//}

//static int EveningGreetingArgs(EveningGreetingArgs eveningArgs)
//{
//    {
//        var drink = ", would you like some wine?.";
//        var noDrink = ".";
//        Console.WriteLine($"{eveningArgs.Greeting} {eveningArgs.Name}{(eveningArgs.Age >= 18 ? drink : noDrink)}");
//    }
//    return 0;
//}
//#endregion

Console.WriteLine("Goodbye from the Sample Console App!");

