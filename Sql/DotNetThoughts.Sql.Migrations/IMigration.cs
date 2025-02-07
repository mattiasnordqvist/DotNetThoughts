using System.Data.Common;

namespace DotNetThoughts.Sql.Migrations;

/// <summary>
/// Represents a migration that can be executed against a database.
/// A migrations version should be unique and all migrations versions should be ordered in the order that they should be executed.
/// </summary>
public interface IMigration
{
    long Version { get; }
    string Name { get; }
    bool IsSnapshot { get; }
    public void Execute(DbConnection c, DbTransaction t, int commandTimeout);
}
