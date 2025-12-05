    using DragonFruit2;
    using SampleConsoleApp;

    var myArgs = Cli.ParseArgs<MyArgs>(args);

    if (myArgs is not  null)
    {
        var drink = ", would you like some wine?.";
        var noDrink = ".";
        Console.WriteLine($"{myArgs.Greeting} {myArgs.Name}{(myArgs.Age >= 18 ? drink : noDrink)}");
    }
