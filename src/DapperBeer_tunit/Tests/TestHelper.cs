namespace DapperBeer.Tests;

public class TestHelper
{
    [Before(TestSession)]
    public static void CreateAndPopulateDatabase()
    {
        var task = DbHelper.CreateTablesAndInsertData();
        task.Wait();
    }   
}