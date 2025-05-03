
using Bogus;

using Dapper;

using DotNetThoughts.Sql.Utilities;

using Microsoft.Data.SqlClient;

using Testcontainers.MsSql;

namespace DotNetThoughts.Sql.Inspection.Tests;
public class Tests
{
    public static string? _masterConnectionString = null;

    [Before(Class)]
    public static async Task CreateSqlServer(CancellationToken cancellationToken)
    {
        var sqlServerContainer = new MsSqlBuilder().Build();
        await sqlServerContainer.StartAsync(cancellationToken);
        _masterConnectionString = sqlServerContainer.GetConnectionString();
    }

    private static string CreateDBForTest()
    {
        var dbNameFaker = new Faker();
        var dbName = $"[{dbNameFaker.Hacker.Adjective() + dbNameFaker.Hacker.Noun()}]";
        using (var connection = new SqlConnection(_masterConnectionString))
        {
            connection.Open();
            connection.Execute("CREATE DATABASE " + dbName);
        }
        var connectionString = ConnectionStringUtils.ReplaceInitialCatalog(_masterConnectionString, dbName);
        return connectionString;
    }

    [Test]
    public async Task TestViewsArePrintedInExecutableOrderWithRegardsToDependencies(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(CreateDBForTest());
        connection.Open();

        await connection.ExecuteAsync("CREATE VIEW A AS SELECT 1 as a;");
        await connection.ExecuteAsync("CREATE VIEW B AS SELECT * FROM A;");
        await connection.ExecuteAsync("CREATE VIEW D AS SELECT * FROM B;");
        await connection.ExecuteAsync("CREATE VIEW C AS SELECT * FROM D;");
        var schema = await Schema.GetSchemaAsync(connection);
        var printed = SqlPrinter.PrintAsExecutable(schema, _ => { });
        await Verify(printed);

    }
    [Test]
    public async Task TestViewsArePrintedAlphabetically(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(CreateDBForTest());
        connection.Open();

        await connection.ExecuteAsync("CREATE VIEW A AS SELECT 1 as a;");
        await connection.ExecuteAsync("CREATE VIEW B AS SELECT 1 as a;");
        await connection.ExecuteAsync("CREATE VIEW D AS SELECT 1 as a;");
        await connection.ExecuteAsync("CREATE VIEW C AS SELECT 1 as a;");
        var schema = await Schema.GetSchemaAsync(connection);
        var printed = SqlPrinter.PrintAsExecutable(schema, _ => { });
        await Verify(printed);

    }
}
