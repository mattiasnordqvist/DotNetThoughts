
using Bogus;

using Dapper;

using DotNetThoughts.Sql.Utilities;

using Microsoft.Data.SqlClient;

using Testcontainers.MsSql;

using static DotNetThoughts.Sql.Inspection.Schema;

namespace DotNetThoughts.Sql.Inspection.Tests;

public class ViewDependencySorterTests
{
    [Test]
    public async Task SortsTopologically()
    {
        var _2 = new DependencyInfo(2, 1);
        var _3 = new DependencyInfo(3, 2);
        var list = new List<DependencyInfo> { _2, _3 };
        var sut = new ViewDependencySorter(list);
        var sortMe = new List<ViewInfo> {
            new(2, "", "", 0),
            new(1, "", "", 0),
            new(3, "", "", 0),
        };
        var sorted = sortMe.OrderBy(x => x, sut).ToList();
        await Assert.That(sorted[0].object_id).IsEqualTo(1);
        await Assert.That(sorted[1].object_id).IsEqualTo(2);
        await Assert.That(sorted[2].object_id).IsEqualTo(3);
    }

    [Test]
    public async Task SortsTopologically2()
    {
        var _5_11 = new DependencyInfo(11, 5);
        var _7_11 = new DependencyInfo(11, 7);
        var _8_7 = new DependencyInfo(8, 7);
        var _8_3 = new DependencyInfo(8, 3);
        var _11_2 = new DependencyInfo(2, 11);
        var _11_9 = new DependencyInfo(9, 11);
        var _8_9 = new DependencyInfo(9, 8);
        var _11_10 = new DependencyInfo(10, 11);
        var _3_10 = new DependencyInfo(10, 3);
        var dependencies = new List<DependencyInfo>
        {
            _5_11, _7_11, _8_7, _8_3, _11_2, _11_9, _8_9, _11_10, _3_10
        };
        var sut = new ViewDependencySorter(dependencies);
        var _5 = new ViewInfo(5, "", "", 0);
        var _11 = new ViewInfo(11, "", "", 0);
        var _2= new ViewInfo(2, "", "", 0);
        var _7 = new ViewInfo(7, "", "", 0);
        var _8 = new ViewInfo(8, "", "", 0);
        var _9 = new ViewInfo(9, "", "", 0);
        var _3 = new ViewInfo(3, "", "", 0);
        var _10 = new ViewInfo(10, "", "", 0);
        var sortMe = new List<ViewInfo>
        {
            _5, _11, _2, _7, _8, _9, _3, _10
        };
        var sorted = sortMe.OrderBy(x => x, sut).ToList();
        await Assert.That(sorted.IndexOf(_5)).IsLessThan(sorted.IndexOf(_11));
        await Assert.That(sorted.IndexOf(_7)).IsLessThan(sorted.IndexOf(_11));
        await Assert.That(sorted.IndexOf(_7)).IsLessThan(sorted.IndexOf(_8));
        await Assert.That(sorted.IndexOf(_3)).IsLessThan(sorted.IndexOf(_8));
        await Assert.That(sorted.IndexOf(_3)).IsLessThan(sorted.IndexOf(_10));
        await Assert.That(sorted.IndexOf(_11)).IsLessThan(sorted.IndexOf(_2));
        await Assert.That(sorted.IndexOf(_11)).IsLessThan(sorted.IndexOf(_9));
        await Assert.That(sorted.IndexOf(_11)).IsLessThan(sorted.IndexOf(_10));
        await Assert.That(sorted.IndexOf(_8)).IsLessThan(sorted.IndexOf(_9));
    }
}
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
        var dbName = dbNameFaker.Hacker.Adjective() + dbNameFaker.Hacker.Noun();
        using (var connection = new SqlConnection(_masterConnectionString))
        {
            connection.Open();
            connection.Execute($"CREATE DATABASE [{dbName}]");
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
