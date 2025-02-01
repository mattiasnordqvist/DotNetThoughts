namespace DotNetThoughts.Sql.Migrations;

/// <summary>
/// Represents a migration that has been applied to the database.
/// </summary>
internal class VersionInfo
{
    public long Version { get; internal set; }
    public string Name { get; internal set; } = null!;
    public bool IsSnapshot { get; internal set; }
}