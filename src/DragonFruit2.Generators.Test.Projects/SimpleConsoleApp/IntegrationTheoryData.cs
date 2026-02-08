namespace SimpleConsoleApp;

public class IntegrationTheoryData: TheoryData<string,string, string, int, string>
{
    public IntegrationTheoryData()
    {
        Add( "NameAgeGreeting", "--name Alice --age 30 --greeting Hello", "Alice", 30, "Hello");
        Add( "Name", "--name Bob", "Bob", 0, "Hi there!" );
        Add("NameGreeting", "--name Charlie --greeting \"Hi there!\"" , "Charlie", 0, "Hi there!");
    }
}