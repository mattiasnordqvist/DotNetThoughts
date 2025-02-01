using Microsoft.Extensions.Options;
namespace DotNetThoughts.Sql.Migrations;

public class MigrationRunnerConfiguration<T> where T : MigrationRunner<T>
{
    public MigrationRunnerConfiguration(IOptions<MigrationRunnerOptions<T>> options)
    {
        Options = options;
    }
    public IOptions<MigrationRunnerOptions<T>> Options { get; private set; }

    internal List<IMigrationLoader> MigrationLoaders => _migrationLoaders;
    private readonly List<IMigrationLoader> _migrationLoaders = [];

    /// <summary>
    /// Adds a migration loader to the migration runner. Migration loaders are responsible for loading the migrations from a source. Without any migration loaders, the migration runner will not be able to find any migrations to run.
    /// When using multiple migration loaders, make sure that migration versions are unique across all loaders.
    /// </summary>
    public void AddMigrationLoaders(IEnumerable<IMigrationLoader> migrationLoaders)
    {
        _migrationLoaders.AddRange(migrationLoaders);
    }

    /// <summary>
    /// Adds a migration loader to the migration runner. Migration loaders are responsible for loading the migrations from a source. Without any migration loaders, the migration runner will not be able to find any migrations to run.
    /// When using multiple migration loaders, make sure that migration versions are unique across all loaders.
    /// </summary>
    public void AddMigrationLoaders(params IMigrationLoader[] migrationLoaders)
    {
        _migrationLoaders.AddRange(migrationLoaders);
    }
}
