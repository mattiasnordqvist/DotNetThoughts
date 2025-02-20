namespace DotNetThoughts.Sql.Migrations.Tests;
public class FakeMigrationLoader : List<IMigration>, IMigrationLoader
{
    public IEnumerable<IMigration> LoadMigrations()
    {
        return this;
    }
}
