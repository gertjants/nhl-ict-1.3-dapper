using System.Data;
using MySqlConnector;

namespace ExampleFromSheets;

public static class DbHelper
{
    public static IDbConnection GetConnection()
    {
        return new MySqlConnection("server=localhost;database=sakila;user=root;password=Test@1234!");
    }
}