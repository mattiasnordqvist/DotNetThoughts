using DotNetThoughts.Results;

using System.Reflection;

namespace DotNetThoughts.Sql.Migrations;

/// <summary>
/// Loads migrations from the embedded resources of the specified assemblies.
/// 
/// Each migration is expected to have a name that begins with 4 groups of 3 digits each, with the groups separated by underscores, like this: "000_000_001_001_migration_name.sql"
/// Each migration is expected to contain valid sql. 
/// The sql can contain multiple batches separated by "GO" or "GO;", appearing on their own lines.
/// </summary>
public class EmbeddedResourceMigrationLoader : IMigrationLoader
{
    private readonly Assembly[] _assemblieWhereYourMigrationsAreEmbedded;

    public EmbeddedResourceMigrationLoader(params Assembly[] assemblieWhereYourMigrationsAreEmbedded)
    {
        _assemblieWhereYourMigrationsAreEmbedded = assemblieWhereYourMigrationsAreEmbedded;
        IncludeFilter = static resourceName => resourceName.EndsWith(".sql");
    }

    public Func<string, bool> IncludeFilter { get; set; }

    public Result<IEnumerable<IMigration>> LoadMigrations() => _assemblieWhereYourMigrationsAreEmbedded
            .Select(assembly => (assembly, resources: assembly.GetManifestResourceNames()))
            .SelectMany(tuple => tuple.resources
                .Where(IncludeFilter)
                .Select(resource => EmbeddedResourceMigration.Create(tuple.assembly, resource)));
}
