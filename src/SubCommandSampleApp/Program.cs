using SampleConsoleApp;

internal class Program
{
    private static int Main(string[] args)
    {
        var subcommandCommandDataValues = Cli.ParseArgs<SubCommandArgs>(args);
        Console.WriteLine("Welcome to the SubCommand sample app");

        if (subcommandCommandDataValues.IsValid)
        {
            return subcommandCommandDataValues.Command switch
            {
                MorningCommand morningArgs => MorningGreeting(morningArgs),

                EveningCommand eveningArgs => EveningGreeting(eveningArgs),

                _ => UnknownGreeting()
            };
        }
        else
        {
            subcommandCommandDataValues.ReportErrorsToConsole();
            return 1;
        }

        static int UnknownGreeting()
        {
            Console.WriteLine("What the heck?");
            return 1;
        }

        static int MorningGreeting(MorningCommand morningArgs)
        {
            var breakfast = ", would you like some Cheerios with chocolate milk?.";
            Console.WriteLine($"{morningArgs.Greeting} {morningArgs.Name}{breakfast}");
            return 0;
        }

        static int EveningGreeting(EveningCommand eveningArgs)
        {
            var drink = ", would you like some wine?.";
            var noDrink = ".";
            Console.WriteLine($"{eveningArgs.Greeting} {eveningArgs.Name}{(eveningArgs.Age >= 18 ? drink : noDrink)}");
            return 0;
        }
    }
}