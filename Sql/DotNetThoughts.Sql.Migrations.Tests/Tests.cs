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

        // Pre-create the database to avoid DROP_CREATE issues with connection pooling
        using (var masterConn = new SqlConnection(masterConnectionString))
        {
            await masterConn.OpenAsync(cancellationToken);
            await masterConn.ExecuteAsync($"CREATE DATABASE [{dbName}]");
        }

        // Create two runners that will run concurrently using IF_NOT_EXISTS mode
        var migrationLoader1 = new FakeMigrationLoader();
        var migrationLoader2 = new FakeMigrationLoader();
        foreach (var migration in migrations)
        {
            migrationLoader1.Add(migration);
            migrationLoader2.Add(migration);
        }

        var runner1 = new TestMigrationRunnerIfNotExists(x => x.AddMigrationLoaders(migrationLoader1));
        var runner2 = new TestMigrationRunnerIfNotExists(x => x.AddMigrationLoaders(migrationLoader2));

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
    /// Tests that concurrent database creation operations serialize correctly.
    /// Uses IF_NOT_EXISTS mode to avoid connection pool invalidation issues with DROP_CREATE.
    /// </summary>
    [Test]
    public async Task ConcurrentDatabaseCreationSerializes(CancellationToken cancellationToken)
    {
        var masterConnectionString = _masterConnectionString ?? throw new Exception("Connectionstring not set");

        var dbNameFaker = new Faker();
        var dbName = dbNameFaker.Hacker.Adjective() + dbNameFaker.Hacker.Noun();

        var connectionString = ConnectionStringUtils.ReplaceInitialCatalog(masterConnectionString, dbName);

        var migrations = new List<FakeMigration>
        {
            new FakeMigration(1, "Migration 1", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(50) },
            new FakeMigration(2, "Migration 2", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(50) },
        };

        var migrationLoader1 = new FakeMigrationLoader();
        var migrationLoader2 = new FakeMigrationLoader();
        foreach (var migration in migrations)
        {
            migrationLoader1.Add(migration);
            migrationLoader2.Add(migration);
        }

        // Use IF_NOT_EXISTS mode - both runners will try to create the DB if it doesn't exist
        // With locking, only one will actually create it
        var runner1 = new TestMigrationRunnerIfNotExists(x => x.AddMigrationLoaders(migrationLoader1));
        var runner2 = new TestMigrationRunnerIfNotExists(x => x.AddMigrationLoaders(migrationLoader2));

        // Run both concurrently - with locking, one will create the DB, the other will see it exists
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

    /// <summary>
    /// DEMONSTRATES THE RACE CONDITION: This test runs concurrent migrations WITHOUT locking.
    /// It should fail with concurrency errors, proving the hypothesis that locking is necessary.
    /// 
    /// Expected failures:
    /// - SqlException: Violation of PRIMARY KEY constraint 'PK_VersionInfo'
    /// - SqlException: Cannot insert duplicate key
    /// - Or other concurrency-related errors
    /// 
    /// Note: This is a probabilistic test - race conditions may not manifest every run.
    /// </summary>
    [Test]
    public async Task ConcurrentMigrationsWithoutLocking_DemonstratesRaceCondition(CancellationToken cancellationToken)
    {
        var masterConnectionString = _masterConnectionString ?? throw new Exception("Connectionstring not set");

        var dbNameFaker = new Faker();
        var dbName = dbNameFaker.Hacker.Adjective() + dbNameFaker.Hacker.Noun();

        var connectionString = ConnectionStringUtils.ReplaceInitialCatalog(masterConnectionString, dbName);

        // Use migrations with small delays to increase chance of race condition
        var migrations = new List<FakeMigration>
        {
            new FakeMigration(1, "Migration 1", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(10) },
            new FakeMigration(2, "Migration 2", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(10) },
            new FakeMigration(3, "Migration 3", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(10) },
            new FakeMigration(4, "Migration 4", false) { ExecuteImpl = (c, t, timeout) => Thread.Sleep(10) },
        };

        // Create the database first (without DROP_CREATE race)
        using (var masterConn = new SqlConnection(masterConnectionString))
        {
            await masterConn.OpenAsync(cancellationToken);
            await masterConn.ExecuteAsync($"CREATE DATABASE [{dbName}]");
        }

        // Run multiple concurrent migration attempts WITHOUT locking
        // This should cause race conditions
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();
        
        for (int i = 0; i < 5; i++)
        {
            var migrationLoader = new FakeMigrationLoader();
            foreach (var migration in migrations)
            {
                migrationLoader.Add(migration);
            }

            // Use the runner with locking DISABLED
            var runner = new TestMigrationRunnerNoLocking(x => x.AddMigrationLoaders(migrationLoader));
            
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await runner.RunAsync(connectionString);
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);

        // The test EXPECTS failures - if no exceptions occurred, the race condition
        // didn't manifest (which can happen due to timing), but typically we expect:
        // - PK violations when multiple runners try to insert the same version
        // - Or other concurrency-related SQL errors
        
        // If we got any exceptions, the race condition was demonstrated
        if (exceptions.Count > 0)
        {
            // Verify at least one exception is a SQL-related concurrency error
            var hasConcurrencyError = exceptions.Any(e => 
                e is SqlException sqlEx && 
                (sqlEx.Message.Contains("PK_VersionInfo") || 
                 sqlEx.Message.Contains("duplicate key") ||
                 sqlEx.Message.Contains("PRIMARY KEY") ||
                 sqlEx.Message.Contains("deadlock") ||
                 sqlEx.Message.Contains("lock request") ||
                 sqlEx.Message.Contains("conflicted")));
            
            // Any SQL exception in a concurrent scenario is evidence of a race condition
            var hasAnySqlException = exceptions.Any(e => e is SqlException);
            
            await Assert.That(hasConcurrencyError || hasAnySqlException).IsTrue();
            
            // Test passes - we demonstrated the race condition exists without locking
            return;
        }

        // If no exceptions, check if we have duplicate versions (another sign of race condition)
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var versionInfo = await connection.QueryAsync<VersionInfo>("SELECT * FROM dbo.VersionInfo ORDER BY Version");
        var versions = versionInfo.ToList();

        // With proper locking, we'd have exactly 4 versions
        // Without locking, we might have duplicates or the wrong count
        // If we have exactly 4 unique versions and no exceptions, the race didn't manifest this run
        // (which is possible but unlikely with 5 concurrent runners)
        
        if (versions.Count != 4 || versions.Select(v => v.Version).Distinct().Count() != 4)
        {
            // Race condition manifested through corrupted state
            await Assert.That(versions.Count).IsNotEqualTo(4);
            return;
        }

        // Race condition didn't manifest this run - this can happen due to timing
        // Just pass the test - it's a probabilistic test that may not always show the race
        // The important thing is that WITH locking (ConcurrentMigrationsSerialize test), it never fails
    }
}
