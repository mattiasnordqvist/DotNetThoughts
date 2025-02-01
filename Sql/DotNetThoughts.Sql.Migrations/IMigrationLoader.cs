namespace DotNetThoughts.Sql.Migrations;

/// <summary>
/// Migration loaders are responsible for loading the migrations from a source. Without any migration loaders, the migration runner will not be able to find any migrations to run.
/// </summary>
public interface IMigrationLoader
{
    /// <summary>
    /// Returns a collection of migrations that should be run.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IMigration> LoadMigrations();
}
