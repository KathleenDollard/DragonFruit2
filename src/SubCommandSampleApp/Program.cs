using SampleConsoleApp;

var subcommandArgsDataValues = Cli.ParseArgs<SubCommandArgs>(args);
Console.WriteLine("Welcome to the SubCommand sample app");

if (subcommandArgsDataValues.IsValid)
{
    return subcommandArgsDataValues.Args switch
    {
        MorningArgs morningArgs => MorningGreeting(morningArgs),

        EveningArgs eveningArgs => EveningGreetingArgs(eveningArgs),

        _ => UnknownGreeting()
    }
;
}
else
{
    subcommandArgsDataValues.ReportErrorsToConsole();
}

return 1;

int UnknownGreeting()
{
    Console.WriteLine("What the heck?");
    return 1;
}

Console.WriteLine();

static int MorningGreeting(MorningArgs morningArgs)
{
    var breakfast = ", would you like some Cheerios with chocolate milk?.";
    Console.WriteLine($"{morningArgs.Greeting} {morningArgs.Name}{breakfast}");
    return 0;
}

static int EveningGreetingArgs(EveningArgs eveningArgs)
{
        var drink = ", would you like some wine?.";
        var noDrink = ".";
        Console.WriteLine($"{eveningArgs.Greeting} {eveningArgs.Name}{(eveningArgs.Age >= 18 ? drink : noDrink)}");
    return 0;
}

Console.WriteLine("Goodbye from the Sample Console App!");

