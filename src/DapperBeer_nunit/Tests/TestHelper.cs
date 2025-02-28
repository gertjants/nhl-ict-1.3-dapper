namespace DapperBeer.Tests;
using System.Diagnostics;
using System.Threading.Tasks;

[SetUpFixture]
public class TestHelper
{
    [OneTimeSetUp]
    public static async Task CreateAndPopulateDatabase()
    {
        await DbHelper.CreateTablesAndInsertData();
    }
}