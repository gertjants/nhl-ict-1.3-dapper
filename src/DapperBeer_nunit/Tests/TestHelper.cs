namespace DapperBeer.Tests;

[TestFixture]
public class TestHelper
{
    [OneTimeSetUp]
    public static void CreateAndPopulateDatabase()
    {
        Console.WriteLine("Setting up environment.");
        DbHelper.CreateTablesAndInsertData();
        Console.WriteLine("Done ... Setting up environment.");
        Thread.Sleep(300);
    }   
}