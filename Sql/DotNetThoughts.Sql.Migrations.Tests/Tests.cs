using Bogus;

using Dapper;

using DotNetThoughts.Render;
using DotNetThoughts.Sql.Utilities;

using Microsoft.Data.SqlClient;

using Testcontainers.MsSql;

using static DotNetThoughts.Render.Render;

namespace DotNetThoughts.Sql.Migrations.Tests;
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

    [Test]
    public async Task ValidSeriesOfMigrations(CancellationToken cancellationToken)
    {
        await Test([
            new FakeMigration(1, "Migration 1", false),
            new FakeMigration(2, "Migration 2", false),
            new FakeMigration(3, "Migration 3", false),
            new FakeMigration(4, "Migration 4", false)], cancellationToken);
    }

    [Test]
    public async Task ValidSeriesOfMigrationsWithMajorJumps(CancellationToken cancellationToken)
    {
        await Test([
            new FakeMigration(1, "Migration 1", false),
            new FakeMigration(2, "Migration 2", false),
            new FakeMigration(3, "Migration 3", false),
            new FakeMigration(1000, "Migration 1000", false),
            new FakeMigration(1001, "Migration 1001", false),
            new FakeMigration(1002, "Migration 1002", false),
            new FakeMigration(2000, "Migration 2000", false),
            new FakeMigration(3000, "Migration 3000", false),
            new FakeMigration(3001, "Migration 3001", false),
            new FakeMigration(10000001, "Migration 10000001", false),
            new FakeMigration(10000002, "Migration 10000002", false),
            new FakeMigration(10001000, "Migration 10001000", false),
            new FakeMigration(20000000, "Migration 20000000", false),
            new FakeMigration(30000000, "Migration 30000000", false),
        ], cancellationToken);
    }

    private async Task<Task> Test(IEnumerable<IMigration> migrations, CancellationToken cancellationToken)
    {
        var masterConnectionString = _masterConnectionString ?? throw new Exception("Connectionstring not set");

        var dbNameFaker = new Faker();
        var dbName = dbNameFaker.Hacker.Adjective() + dbNameFaker.Hacker.Noun();

        var connectionString = ConnectionStringUtils.ReplaceInitialCatalog(masterConnectionString, dbName);

        var migrationLoader = new FakeMigrationLoader();
        foreach (var migration in migrations)
        {
            migrationLoader.Add(migration);
        }
        var sut = new TestMigrationRunner(x => x.AddMigrationLoaders(migrationLoader));
        await sut.RunAsync(connectionString);

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var versionInfo = await connection.QueryAsync<VersionInfo>("SELECT * FROM dbo.VersionInfo");
        return Verify(Table.Render([..versionInfo]));

    }
}
