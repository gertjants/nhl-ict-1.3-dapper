using System.Data;
using System.IO;
using Dapper;
using DapperBeer.DTO;
using MySqlConnector;
using System.Transactions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DapperBeer;

public static class DbHelper
{
    private class DatabaseTableInfo
    {
        public required string Table_Name { get; set;}
        public required int Table_Rows { get; set;}
    }
    public static string database = Environment.GetEnvironmentVariable("APP_DB_NAME") ?? "DapperBeer";
    public static string connString = $"server={Environment.GetEnvironmentVariable("APP_DB_SERVER")};database={database};user={Environment.GetEnvironmentVariable("APP_DB_USER")};password={Environment.GetEnvironmentVariable("APP_DB_PASS")};AllowUserVariables=True;";
    public static IDbConnection GetConnection()
        => new MySqlConnection(connString);

    public static async Task CreateTablesAndInsertData()
    {
        using var connection = DbHelper.GetConnection();

        // So the original check has issues due to incorrect amount of record counts.
        // This way everything is only depending on the available data files.
        // Much easier to maintain and much less error prone.
        var actualTables = new List<DatabaseTableInfo>();
        var expectedTables = Directory.GetFiles("./SQL/data").Select(q =>
            new DatabaseTableInfo()
            {
                Table_Name = q[14..^4],
                Table_Rows = File.ReadLines(q).Count(line => !string.IsNullOrWhiteSpace(line))
            });

        var sql = string.Join("\nUNION\n", expectedTables.Select(q =>
            $"SELECT '{q.Table_Name}' as Table_Name, COUNT(1) as Table_Rows FROM `{database}`.`{q.Table_Name}`"));
        
        try
        {
            actualTables = (await connection.QueryAsync<DatabaseTableInfo>(sql)).ToList();
        }
        #pragma warning disable CS0168
        catch(Exception ex)
        {
            // Simple catch. We don't care if this fails.
        }

        var missingTables = expectedTables.Where(q => !actualTables.Any(e => e.Table_Name == q.Table_Name))
                                        .Select(q => q.Table_Name);

        var incorrectTables = expectedTables.Where(q => 
                !actualTables.Any(e => e.Table_Name == q.Table_Name && e.Table_Rows == q.Table_Rows)
                && !missingTables.Contains(q.Table_Name))
            .Select(q => q.Table_Name)
            .Union(missingTables);

        // Tables are missing or incorrect.
        if(missingTables.Any())
            await createTables(missingTables);

        if(incorrectTables.Any())
            await FillTables(incorrectTables);
    }

    public async static Task FlushFillTableReviews()
        => await FillTables(["Review"]);

    private async static Task createTables(IEnumerable<string> tables)
    {
        using var connection = DbHelper.GetConnection();
        // Preamble
        var sql = "";
        sql += $"CREATE DATABASE IF NOT EXISTS `{database}`;";
        sql += $"USE `{database}`;";
        sql += "SET FOREIGN_KEY_CHECKS=0;";
        sql += "SET autocommit=0;"; // If not disabled, a flush to disk PER insert.

        // Getting file list from tables folder
        foreach(var table in Directory.GetFiles("SQL/tables"))
        {
            if(tables.Any(q => table.Contains(q))){
                sql += $"DROP TABLE IF EXISTS `{table[9..^4]}`;";
                sql += File.ReadAllText(table);
                sql += File.ReadAllText($"SQL/data/{table[10..^4]}.sql");
            }
                
        }
        sql += "SET FOREIGN_KEY_CHECKS=1;";
        sql += "SET autocommit=1;";
        await connection.ExecuteAsync(sql);
    }

    private async static Task FillTables(IEnumerable<string> tables)
    {
        using var connection = DbHelper.GetConnection();

        var sql ="";
        // Disabling checks. Much faster to load. Should solve a LOT of perf. problems.
        sql += "SET FOREIGN_KEY_CHECKS=0;";
        sql += "SET autocommit=0;"; // If not disabled, a flush to disk PER insert. Waste of performance.

        foreach(var table in Directory.GetFiles("SQL/data"))
        {
            if(tables.Any(q => table.Contains(q))){
                sql += $"TRUNCATE TABLE `{database}`.`{table[12..^4]}`;";
                sql += File.ReadAllText($"SQL/data/{table[9..^4]}.sql");
                sql += "COMMIT;"; // Flush to disk per table. See MySQL documentation.
            }
        }

        // Re-enabling checks.
        sql += "SET FOREIGN_KEY_CHECKS=1;";
        sql += "SET autocommit=1;";
        await connection.ExecuteAsync(sql);
    }
}