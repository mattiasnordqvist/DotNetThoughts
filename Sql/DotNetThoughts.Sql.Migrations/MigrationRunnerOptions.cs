namespace DotNetThoughts.Sql.Migrations;

public class MigrationRunnerOptions<T> where T : MigrationRunner<T>
{

    public bool EnableSnapshot { get; set; } = false;
    /// <summary>
    /// The command timeout in seconds for database operations. Default is 200 seconds.
    /// </summary>
    public int DefaultCommandTimeout { get; set; } = 200;

    /// <summary>
    /// The name of the database table that will store the migration history.
    /// </summary>
    public string VersionInfoTableName { get; set; } = "VersionInfo";

    /// <summary>
    /// The schema of the database table that will store the migration history.
    /// </summary>
    public string VersionInfoTableSchema { get; set; } = "dbo";

    /// <summary>
    ///  Configure the Migration Runner to either
    /// 1. Create the database IF_NOT_EXISTS already.
    /// 2. To DROP_AND_CREATE the database, even if it already exists, or creates the database if it does not exist.
    /// 3. NEVER create the database. Expect it to exist already.
    /// Default to NEVER.
    /// </summary>
    public AutoCreateMode AutoCreate { get; set; } = AutoCreateMode.NEVER;


    /// <summary>
    /// If set to true, and AutoCreate is either CREATE_IF_NOT_EXISTS or DROP_AND_CREATE, the database will not be created empty, but instead copied from another database.
    /// Default to false. If set to true, be sure to set SourceDatabaseNameForBackups as well.
    /// </summary>
    public bool RestoreFromDatabaseOnAutoCreate { get; set; } = false;

    /// <summary>
    /// If the RestoreFromDatabaseOnAutoCreate is set to true, this is the name of the database to restore from.
    /// </summary>
    public string? SourceDatabaseForRestore { get; set; } = null;

}
