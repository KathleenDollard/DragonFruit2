using SampleConsoleApp;

internal class Program
{
    private static int Main(string[] args)
    {
        var subcommandCommandDataValues = Cli.ParseArgs<SubCommandCommand>(args);
        Console.WriteLine("Welcome to the SubCommand sample app");

        if (subcommandCommandDataValues.IsValid)
        {
            return subcommandCommandDataValues.Command switch
            {
                MorningCommand morningCommand => MorningGreeting(morningCommand),

                EveningCommand eveningCommand => EveningGreeting(eveningCommand),

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
            Console.WriteLine("What the heck? The command was not found, perhaps you forgot the CommandClass attribute?");
            return 1;
        }

        static int MorningGreeting(MorningCommand morningCommand)
        {
            var breakfast = ", would you like some Cheerios with chocolate milk?.";
            Console.WriteLine($"{morningCommand.Greeting} {morningCommand.Name}{breakfast}");
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