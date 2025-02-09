namespace DapperBeer.Tests;

public class TestHelper
{
    [Before(Class)]
    public static void CreateAndPopulateDatabase()
    {
        DbHelper.CreateTablesAndInsertData();
    }   
}