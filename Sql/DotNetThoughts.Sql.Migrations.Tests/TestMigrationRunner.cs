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
