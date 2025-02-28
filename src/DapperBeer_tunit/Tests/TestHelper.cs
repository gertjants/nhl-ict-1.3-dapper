namespace DapperBeer.Tests;

public class TestHelper
{
    [Before(Class)]
    public static void CreateAndPopulateDatabase()
    {
        var task = DbHelper.CreateTablesAndInsertData();
        task.Wait();
    }   
}