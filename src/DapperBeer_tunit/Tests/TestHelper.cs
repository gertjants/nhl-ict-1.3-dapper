namespace DapperBeerTunit.Tests;
using DapperBeer;

public class TestHelper
{
    [Before(TestSession)]
    public static void CreateAndPopulateDatabase()
    {
        var task = DbHelper.CreateTablesAndInsertData();
        task.Wait();
    }   
}