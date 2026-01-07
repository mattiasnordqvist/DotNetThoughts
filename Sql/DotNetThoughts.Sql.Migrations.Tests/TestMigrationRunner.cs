using FakeItEasy;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetThoughts.Sql.Migrations.Tests;

public class TestMigrationRunner : MigrationRunner<TestMigrationRunner>
{
    public static MigrationRunnerConfiguration<TestMigrationRunner> DefaultConfiguration(Action<MigrationRunnerConfiguration<TestMigrationRunner>> configure)
    {
        var configuration = new MigrationRunnerConfiguration<TestMigrationRunner>(Options.Create(new MigrationRunnerOptions<TestMigrationRunner>
        {
            AutoCreate = AutoCreateMode.DROP_CREATE,
            EnableSnapshot = false
        }));
        configure(configuration);
        return configuration;
    }

    public TestMigrationRunner(Action<MigrationRunnerConfiguration<TestMigrationRunner>> configure)
        : base(DefaultConfiguration(configure), A.Fake<ILogger<TestMigrationRunner>>())
    {
    }
}

public class TestMigrationRunnerWithShortTimeout : MigrationRunner<TestMigrationRunnerWithShortTimeout>
{
    public static MigrationRunnerConfiguration<TestMigrationRunnerWithShortTimeout> DefaultConfiguration(Action<MigrationRunnerConfiguration<TestMigrationRunnerWithShortTimeout>> configure)
    {
        var configuration = new MigrationRunnerConfiguration<TestMigrationRunnerWithShortTimeout>(Options.Create(new MigrationRunnerOptions<TestMigrationRunnerWithShortTimeout>
        {
            AutoCreate = AutoCreateMode.NEVER, // Database already exists
            EnableSnapshot = false,
            MigrationLockTimeoutMs = 100 // Very short timeout for testing
        }));
        configure(configuration);
        return configuration;
    }

    public TestMigrationRunnerWithShortTimeout(Action<MigrationRunnerConfiguration<TestMigrationRunnerWithShortTimeout>> configure)
        : base(DefaultConfiguration(configure), A.Fake<ILogger<TestMigrationRunnerWithShortTimeout>>())
    {
    }
}

/// <summary>
/// Migration runner with IF_NOT_EXISTS mode - used for concurrent creation tests.
/// </summary>
public class TestMigrationRunnerIfNotExists : MigrationRunner<TestMigrationRunnerIfNotExists>
{
    public static MigrationRunnerConfiguration<TestMigrationRunnerIfNotExists> DefaultConfiguration(Action<MigrationRunnerConfiguration<TestMigrationRunnerIfNotExists>> configure)
    {
        var configuration = new MigrationRunnerConfiguration<TestMigrationRunnerIfNotExists>(Options.Create(new MigrationRunnerOptions<TestMigrationRunnerIfNotExists>
        {
            AutoCreate = AutoCreateMode.IF_NOT_EXISTS,
            EnableSnapshot = false
        }));
        configure(configuration);
        return configuration;
    }

    public TestMigrationRunnerIfNotExists(Action<MigrationRunnerConfiguration<TestMigrationRunnerIfNotExists>> configure)
        : base(DefaultConfiguration(configure), A.Fake<ILogger<TestMigrationRunnerIfNotExists>>())
    {
    }
}

/// <summary>
/// Migration runner with locking DISABLED - used to demonstrate race conditions.
/// </summary>
public class TestMigrationRunnerNoLocking : MigrationRunner<TestMigrationRunnerNoLocking>
{
    public static MigrationRunnerConfiguration<TestMigrationRunnerNoLocking> DefaultConfiguration(Action<MigrationRunnerConfiguration<TestMigrationRunnerNoLocking>> configure)
    {
        var configuration = new MigrationRunnerConfiguration<TestMigrationRunnerNoLocking>(Options.Create(new MigrationRunnerOptions<TestMigrationRunnerNoLocking>
        {
            AutoCreate = AutoCreateMode.NEVER, // DB pre-created in test
            EnableSnapshot = false,
            UseMigrationLock = false // LOCKING DISABLED - will cause race conditions!
        }));
        configure(configuration);
        return configuration;
    }

    public TestMigrationRunnerNoLocking(Action<MigrationRunnerConfiguration<TestMigrationRunnerNoLocking>> configure)
        : base(DefaultConfiguration(configure), A.Fake<ILogger<TestMigrationRunnerNoLocking>>())
    {
    }
}
