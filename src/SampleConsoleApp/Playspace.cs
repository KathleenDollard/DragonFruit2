namespace SampleConsoleApp;

// Source - https://stackoverflow.com/a
// Posted by canton7, modified by community. See post 'Timeline' for change history
// Retrieved 2025-11-14, License - CC BY-SA 4.0

interface IAnimal
{
    static abstract string Sound { get; }
}

class Dog : IAnimal
{
    public static string Sound => "woof";
}

class Cat : IAnimal
{
    public static string Sound => "meow";
}

class AnimalSoundExplainer<T> where T : IAnimal
{

    public static void Explain() =>
        Console.WriteLine("Every " + typeof(T).Name + " makes " + T.Sound);
}
