using Bogus;

using Dapper;

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
        return Verify(Table.Render([.. versionInfo]));

    }

    /// <summary>
    /// Tests that concurrent migrations serialize correctly using application locks.
    /// Two runners should not cause PK_VersionInfo violations.
    /// </summary>
    [Test]
    public async Task ConcurrentMigrationsSerialize(CancellationToken cancellationToken)
    {
        var masterConnectionString = _masterConnectionString ?? throw new Exception("Connectionstring not set");

        var dbNameFaker = new Faker();
        var dbName = dbNameFaker.Hacker.Adjective() + dbNameFaker.Hacker.Noun();

        var connectionString = ConnectionStringUtils.ReplaceInitialCatalog(masterConnectionString, dbName);

        var migrations = new List<FakeMigration>
        {
            new FakeMigration(1, "Migration 1", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(100) },
            new FakeMigration(2, "Migration 2", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(100) },
            new FakeMigration(3, "Migration 3", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(100) },
            new FakeMigration(4, "Migration 4", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(100) },
        };

        // Create two runners that will run concurrently
        var migrationLoader1 = new FakeMigrationLoader();
        var migrationLoader2 = new FakeMigrationLoader();
        foreach (var migration in migrations)
        {
            migrationLoader1.Add(migration);
            migrationLoader2.Add(migration);
        }

        var runner1 = new TestMigrationRunner(x => x.AddMigrationLoaders(migrationLoader1));
        var runner2 = new TestMigrationRunner(x => x.AddMigrationLoaders(migrationLoader2));

        // Run both concurrently - with locking, both should complete without errors
        var task1 = runner1.RunAsync(connectionString);
        var task2 = runner2.RunAsync(connectionString);

        await Task.WhenAll(task1, task2);

        // Verify results - each version should appear exactly once
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var versionInfo = await connection.QueryAsync<VersionInfo>("SELECT * FROM dbo.VersionInfo ORDER BY Version");
        var versions = versionInfo.ToList();

        await Assert.That(versions.Count).IsEqualTo(4);
        await Assert.That(versions.Select(v => v.Version)).IsEquivalentTo([1L, 2L, 3L, 4L]);
    }

    /// <summary>
    /// Tests that concurrent DROP_CREATE operations serialize correctly.
    /// </summary>
    [Test]
    public async Task ConcurrentDropCreateSerializes(CancellationToken cancellationToken)
    {
        var masterConnectionString = _masterConnectionString ?? throw new Exception("Connectionstring not set");

        var dbNameFaker = new Faker();
        var dbName = dbNameFaker.Hacker.Adjective() + dbNameFaker.Hacker.Noun();

        var connectionString = ConnectionStringUtils.ReplaceInitialCatalog(masterConnectionString, dbName);

        var migrations = new List<FakeMigration>
        {
            new FakeMigration(1, "Migration 1", false),
            new FakeMigration(2, "Migration 2", false),
        };

        var migrationLoader1 = new FakeMigrationLoader();
        var migrationLoader2 = new FakeMigrationLoader();
        foreach (var migration in migrations)
        {
            migrationLoader1.Add(migration);
            migrationLoader2.Add(migration);
        }

        // Both use DROP_CREATE mode (default in TestMigrationRunner)
        var runner1 = new TestMigrationRunner(x => x.AddMigrationLoaders(migrationLoader1));
        var runner2 = new TestMigrationRunner(x => x.AddMigrationLoaders(migrationLoader2));

        // Run both concurrently - with locking, one will complete first, then the other
        // will drop and recreate, but without race conditions
        var task1 = runner1.RunAsync(connectionString);
        var task2 = runner2.RunAsync(connectionString);

        await Task.WhenAll(task1, task2);

        // Verify the database exists and is properly migrated
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var versionInfo = await connection.QueryAsync<VersionInfo>("SELECT * FROM dbo.VersionInfo ORDER BY Version");
        var versions = versionInfo.ToList();

        await Assert.That(versions.Count).IsEqualTo(2);
        await Assert.That(versions.Select(v => v.Version)).IsEquivalentTo([1L, 2L]);
    }

    /// <summary>
    /// Tests that lock timeout throws MigrationLockException with helpful message.
    /// </summary>
    [Test]
    public async Task LockTimeoutThrowsMigrationLockException(CancellationToken cancellationToken)
    {
        var masterConnectionString = _masterConnectionString ?? throw new Exception("Connectionstring not set");

        var dbNameFaker = new Faker();
        var dbName = dbNameFaker.Hacker.Adjective() + dbNameFaker.Hacker.Noun();

        var connectionString = ConnectionStringUtils.ReplaceInitialCatalog(masterConnectionString, dbName);

        // First, create the database so we can acquire a lock on it
        using (var masterConn = new SqlConnection(masterConnectionString))
        {
            await masterConn.OpenAsync(cancellationToken);
            await masterConn.ExecuteAsync($"CREATE DATABASE [{dbName}]");
        }

        // Open a connection and hold a lock
        using var lockConnection = new SqlConnection(connectionString);
        await lockConnection.OpenAsync(cancellationToken);

        var lockResource = $"DotNetThoughts.Sql.Migrations:RunMigrations:{dbName}";
        await lockConnection.ExecuteAsync(
            "EXEC sp_getapplock @Resource = @LockResource, @LockMode = 'Exclusive', @LockOwner = 'Session', @LockTimeout = 60000",
            new { LockResource = lockResource });

        try
        {
            // Now try to run migrations with a very short timeout
            var migrations = new List<FakeMigration>
            {
                new FakeMigration(1, "Migration 1", false),
            };

            var migrationLoader = new FakeMigrationLoader();
            foreach (var migration in migrations)
            {
                migrationLoader.Add(migration);
            }

            var runner = new TestMigrationRunnerWithShortTimeout(x => x.AddMigrationLoaders(migrationLoader));

            // This should throw MigrationLockException
            var exception = await Assert.ThrowsAsync<MigrationLockException>(
                async () => await runner.RunAsync(connectionString));

            await Assert.That(exception).IsNotNull();
            await Assert.That(exception!.DatabaseName).IsEqualTo(dbName);
            await Assert.That(exception.LockResource).Contains("RunMigrations");
            await Assert.That(exception.TimeoutMs).IsEqualTo(100);
            await Assert.That(exception.Message).Contains("Failed to acquire migration lock");
        }
        finally
        {
            // Release the lock
            await lockConnection.ExecuteAsync(
                "EXEC sp_releaseapplock @Resource = @LockResource, @LockOwner = 'Session'",
                new { LockResource = lockResource });
        }
    }
}
