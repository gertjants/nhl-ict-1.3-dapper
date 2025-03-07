namespace DapperBeerNunit.Tests;
using System.Diagnostics;
using System.Threading.Tasks;
using DapperBeer;

[SetUpFixture]
public class TestHelper
{
    [OneTimeSetUp]
    public static async Task CreateAndPopulateDatabase()
    {
        await DbHelper.CreateTablesAndInsertData();
    }
}