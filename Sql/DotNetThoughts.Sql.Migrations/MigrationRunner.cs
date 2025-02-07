using Dapper;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using System.Data.Common;
using DotNetThoughts.Sql.Utilities;
using DotNetThoughts.Results;
namespace DotNetThoughts.Sql.Migrations;

public abstract class MigrationRunner<T>(MigrationRunnerConfiguration<T> configuration, ILogger<T> logger) where T : MigrationRunner<T>
{
    private readonly MigrationRunnerConfiguration<T> _configuration = configuration;
    private readonly ILogger<T> _logger = logger;

    public async Task RunAsync(string connectionString)
    {
        var autoCreate = _configuration.Options.Value.AutoCreate;

        _logger.LogInformation("AutoCreate database {autoCreate} configured!", autoCreate);

        if (autoCreate == AutoCreateMode.IF_NOT_EXISTS || autoCreate == AutoCreateMode.DROP_CREATE)
        {
            var masterConnStr = ConnectionStringUtils.ReplaceInitialCatalog(connectionString, "master");
            var localDb = ConnectionStringUtils.GetInitialCatalog(connectionString);
            await CreateDatabase(masterConnStr, localDb, autoCreate == AutoCreateMode.DROP_CREATE);
        }

        if (_configuration.MigrationLoaders.Count == 0)
        {
            _logger.LogWarning("No migration loaders configured! No migrations will be run.");
            return;
        }

        var loadMigrationsResults = _configuration.MigrationLoaders.Select(x => x.LoadMigrations()).ToList();
        var migrations = new List<IMigration>();
        var actualErrors = UnitResult.Ok;
        foreach(var loadMigrationsResult in loadMigrationsResults)
        {
            if (loadMigrationsResult.Success)
            {
                migrations.AddRange(loadMigrationsResult.Value);
            }
            else
            {
                foreach(var error in loadMigrationsResult.Errors)
                {
                    if(error is LoaderFoundNoMigrationsError loaderFoundNoMigrationsError)
                    {
                        _logger.LogWarning(loaderFoundNoMigrationsError.Message);
                    }
                    else
                    {
                        _logger.LogError(error.Message);
                        actualErrors = actualErrors.Or(UnitResult.Error(error));
                    }
                }
            }
        }

        actualErrors.ValueOrThrow();

        if (migrations.Count == 0)
        {
            _logger.LogWarning("No migrations found.");
            return;
        }

        OfflineMigrationsValidation(migrations, _logger, true);
        using var connection = new SqlConnection(connectionString);
        connection.Open();


        OnlineMigrationsValidation(migrations, await GetVersionInfo(connection), true);

        var lastMigrationVersion = await GetCurrentVersion(connection);
        List<IMigration> migrationsToExecute = [];
        if (lastMigrationVersion == 0 && _configuration.Options.Value.EnableSnapshot)
        {
            var lastSnapshot = migrations
                .Where(x => x.IsSnapshot)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();
            if (lastSnapshot != null)
            {
                migrationsToExecute.Add(lastSnapshot);
                lastMigrationVersion = lastSnapshot.Version;
            }
        }
        migrationsToExecute.AddRange(migrations
            .Where(x => !x.IsSnapshot)
            .Where(x => x.Version > lastMigrationVersion)
            .OrderBy(x => x.Version));

        foreach (IMigration m in migrationsToExecute)
        {
            using var t = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
            try
            {
                var sql = $"INSERT INTO {_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName} (Version, IsSnapshot, Name, AppliedAt) VALUES (@Version, @IsSnapshot, @Name, @AppliedAt)";
                await connection.ExecuteAsync(sql, new { m.Version, m.IsSnapshot, m.Name, AppliedAt = DateTimeOffset.Now }, transaction: t);
                m.Execute(connection, t, commandTimeout: _configuration.Options.Value.DefaultCommandTimeout);
                t.Commit();
                _logger.LogInformation("Applied {version}: {name}", m.Version, m.Name);
            }
            catch
            {
                _logger.LogError("Failed to apply migration {version}: {name}", m.Version, m.Name);
                throw;
            }
        }
        _logger.LogInformation("Database up to date! Version {version}", migrationsToExecute.Count != 0 ? migrationsToExecute.Last().Version : lastMigrationVersion);
    }



    private Task<bool> DatabaseExists(SqlConnection connection, string databaseName)
    {
        var sql = @$"SELECT CASE WHEN EXISTS(SELECT * FROM sys.databases WHERE name = '{databaseName}') THEN 1 ELSE 0 END";
        return connection.ExecuteScalarAsync<bool>(sql);
    }
    private async Task CreateDatabase(string masterConnStr, string databaseName, bool dropIfAlreadyExists)
    {
        var masterConnection = new SqlConnection(masterConnStr);
        await masterConnection.OpenAsync();

        if (dropIfAlreadyExists && await DatabaseExists(masterConnection, databaseName))
        {
            _logger.LogInformation("Database {database} already exists but is configured to be recreated!", databaseName);
            _logger.LogInformation("Dropping database {database}!", databaseName);
            var sql = $@"
                ALTER DATABASE {databaseName.ToIdentifier()} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE {databaseName.ToIdentifier()};";
            await masterConnection.ExecuteAsync(sql);
        }

        if (await DatabaseExists(masterConnection, databaseName))
        {
            _logger.LogInformation("Database {database} already exists!", databaseName);
        }
        else
        {
            _logger.LogInformation("Database {database} does not exist.", databaseName);
            try
            {
                if (!_configuration.Options.Value.RestoreFromDatabaseOnAutoCreate)
                {
                    _logger.LogInformation("Creating new database {databaseName}", databaseName);
                    var sql = $"CREATE DATABASE {databaseName.ToIdentifier()};";
                    await masterConnection.ExecuteAsync(sql);
                }
                else
                {
                    var sourceDatabase = _configuration.Options.Value.SourceDatabaseForRestore ?? throw new Exception("Source database for restore not configured. When RestoreFromBackupOnAutoCreate is true, a source database must exist.");
                    if (!await DatabaseExists(masterConnection, sourceDatabase))
                    {
                        throw new Exception($"Source database {sourceDatabase} does not exist! Can't create copy a non-existing database. Make sure the source database exists.");
                    }
                    _logger.LogInformation("{databaseName} does not exist. Restoring from {sourceDatabase}.", databaseName, sourceDatabase);
                    var sql = $"""
                    DECLARE @BackupFolderPath sql_variant = (SELECT ServerProperty(N'InstanceDefaultBackupPath'));
                    DECLARE @DataFolderPath sql_variant = (SELECT ServerProperty(N'InstanceDefaultDataPath'));
                    DECLARE @LogFolderPath sql_variant = (SELECT ServerProperty(N'InstanceDefaultLogPath'));
                    DECLARE @BackupPath VARCHAR(2000) = CONVERT(VARCHAR(1000), @BackupFolderPath) + '\{sourceDatabase}-migrationrunnerbackup.bak';
                    IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{databaseName}')
                    BEGIN
                        CREATE DATABASE {databaseName.ToIdentifier()};

                        BACKUP DATABASE {sourceDatabase.ToIdentifier()} TO  DISK = @BackupPath WITH COPY_ONLY, INIT;

                        DECLARE @DataPath VARCHAR(2000) = CONVERT(VARCHAR(1000), @DataFolderPath) + '\{databaseName}.mdf';
                        DECLARE @LogPath VARCHAR(2000) = CONVERT(VARCHAR(1000), @LogFolderPath) + '\{databaseName}_log.ldf';

                                    
                        DECLARE @fileListTable TABLE (
                            [LogicalName]           NVARCHAR(128),
                            [PhysicalName]          NVARCHAR(260),
                            [Type]                  CHAR(1),
                            [FileGroupName]         NVARCHAR(128),
                            [Size]                  NUMERIC(20,0),
                            [MaxSize]               NUMERIC(20,0),
                            [FileID]                BIGINT,
                            [CreateLSN]             NUMERIC(25,0),
                            [DropLSN]               NUMERIC(25,0),
                            [UniqueID]              UNIQUEIDENTIFIER,
                            [ReadOnlyLSN]           NUMERIC(25,0),
                            [ReadWriteLSN]          NUMERIC(25,0),
                            [BackupSizeInBytes]     BIGINT,
                            [SourceBlockSize]       INT,
                            [FileGroupID]           INT,
                            [LogGroupGUID]          UNIQUEIDENTIFIER,
                            [DifferentialBaseLSN]   NUMERIC(25,0),
                            [DifferentialBaseGUID]  UNIQUEIDENTIFIER,
                            [IsReadOnly]            BIT,
                            [IsPresent]             BIT,
                            [TDEThumbprint]         VARBINARY(32),
                            [SnapshotURL]           NVARCHAR(360) 
                        )
                        INSERT INTO @fileListTable EXEC('RESTORE FILELISTONLY FROM DISK ='''+@BackupPath+'''')
                        DECLARE @LogicalSourceDataName VARCHAR(1000) = (SELECT LogicalName FROM @fileListTable WHERE Type = 'D')
                        DECLARE @LogicalSourceLogName VARCHAR(1000) = (SELECT LogicalName FROM @fileListTable WHERE Type = 'L')

                        EXEC('RESTORE DATABASE {databaseName.ToIdentifier()} FROM  DISK = '''+@BackupPath+''' WITH  
                      		    FILE = 1,  
                      		    MOVE '''+@LogicalSourceDataName+''' TO '''+@DataPath+''',  
                      		    MOVE '''+@LogicalSourceLogName+''' TO '''+@LogPath+''',  
                      		    NOUNLOAD,  
                      		    REPLACE,  
                      		    STATS = 5;')
                    END
                    """;
                    try
                    {
                        await masterConnection.ExecuteAsync(sql);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to restore database {databaseName}", databaseName);
                        throw;
                    }
                }
            }
            finally
            {
                masterConnection.Close();
                masterConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Creates the VersionInfo table if it does not exist. 
    /// The VersionInfo table is used to keep track of which migrations have been applied to the database.
    /// </summary>
    private async Task CreateVersionInfoTableIfNotExists(DbConnection dbConnection)
    {
        var sql = $"""
            -- Version 1
            IF (NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{_configuration.Options.Value.VersionInfoTableSchema.ToIdentifier().Regular}' AND TABLE_NAME = '{_configuration.Options.Value.VersionInfoTableName.ToIdentifier().Regular}')) 
            BEGIN 
                CREATE TABLE {_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName}([Version] BIGINT NOT NULL, Name NVARCHAR(255) NOT NULL, AppliedAt DATETIMEOFFSET NOT NULL) 
                ALTER TABLE {_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName} ADD CONSTRAINT PK_{_configuration.Options.Value.VersionInfoTableName.ToIdentifier().Regular} PRIMARY KEY ([Version])
            END
            GO

            -- Version 2 - Add IsSnapshot column
            IF (NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'IsSnapshot' AND Object_ID = Object_ID(N'{_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName}')))
            BEGIN

                ALTER TABLE {_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName} ADD IsSnapshot BIT NULL
            END
            GO
            
            IF ((SELECT is_nullable FROM sys.columns WHERE Name = N'IsSnapshot' AND Object_ID = Object_ID(N'{_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName}')) = 1)
            BEGIN
                UPDATE {_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName} SET IsSnapshot = 0 WHERE IsSnapshot IS NULL
                ALTER TABLE {_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName} ADD CONSTRAINT [DF_{_configuration.Options.Value.VersionInfoTableName.ToIdentifier().Regular}_IsSnapshot] DEFAULT ((0)) FOR [IsSnapshot];
                ALTER TABLE {_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName} ALTER COLUMN IsSnapshot BIT NOT NULL
            END
            """;

        foreach (var statement in Utilities.BatchSplit(sql))
        {
            await dbConnection.ExecuteAsync(statement);
        }
    }

    /// <summary>
    /// Returns a list of version info that each represents a migration that has been applied to the database.
    /// </summary>
    private async Task<List<VersionInfo>> GetVersionInfo(DbConnection dbConnection)
    {
        await CreateVersionInfoTableIfNotExists(dbConnection);
        var l = await dbConnection.QueryAsync<VersionInfo>($"SELECT Version, IsSnapshot, Name FROM {_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName}");
        return l.ToList();
    }

    /// <summary>
    /// Returns the curren version of the database. If no migrations have been applied, 0 is returned.
    /// </summary>
    private async Task<long> GetCurrentVersion(DbConnection dbConnection)
    {
        await CreateVersionInfoTableIfNotExists(dbConnection);
        var l = await dbConnection.ExecuteScalarAsync<long>($"SELECT ISNULL(MAX(Version),0) FROM {_configuration.Options.Value.VersionInfoTableSchema}.{_configuration.Options.Value.VersionInfoTableName}");
        return l;
    }

    /// <summary>
    /// Checks that several assumptions about the migrations are true. If not, logs errors and throws an exception if throwExceptionOnValidationFailure is true.
    /// Only assumptions that can be checked without a database connection are checked here. The rest are checked in OnlineMigrationsValidation.
    /// This method checks the following:
    /// 1. There must be at least one migration.
    /// 2. There must not be multiple migrations with the same version number.
    /// 3. The migrations must be sequential. Migrations must be sequential within the same 1000s. 1000, 1001, 1002 is a valid sequence. 1000, 1002 is not. 1000, 1001, 2000 is valid as well, since 2000 is in a different 1000-part.
    /// </summary>
    /// <param name="migrations">The migrations to validate</param>
    /// <param name="logger"></param>
    /// <param name="throwExceptionOnValidationFailure"></param>
    /// <exception cref="Exception"></exception>
    public static void OfflineMigrationsValidation(IEnumerable<IMigration> migrations, ILogger? logger, bool throwExceptionOnValidationFailure = true)
    {
        if (migrations.Count() == 0)
        {
            var message = "No migrations found!";
            logger?.LogError(message);
            if (throwExceptionOnValidationFailure)
            {
                throw new Exception(message);
            }
        }
        var conflictingMigrations = migrations.GroupBy(x => (x.Version, x.IsSnapshot)).Where(x => x.Count() > 1).ToList();
        if (conflictingMigrations.Any())
        {
            var message = $"""
                Multiple migrations with version number: 
                {string.Join(Environment.NewLine, conflictingMigrations.SelectMany(x => x.Select(y => $"{y.Version}: {y.Name}{(y.IsSnapshot ? " (snapshot)" : "")}")))}

                Please fix this before continuing.
                """;
            logger?.LogError(message);
            if (throwExceptionOnValidationFailure)
            {
                throw new Exception(message);
            }
        }

        var orderedMigrations = migrations.Where(x => !x.IsSnapshot).OrderBy(x => x.Version).ToList();
        for (int i = 1; i < orderedMigrations.Count(); i++)
        {
            if (orderedMigrations[i].Version / 1000 == orderedMigrations[i - 1].Version / 1000 && orderedMigrations[i].Version != orderedMigrations[i - 1].Version + 1)
            {
                var message = $"Migrations are not sequential. {orderedMigrations[i - 1].Version} followed by {orderedMigrations[i].Version}";
                logger?.LogError(message);
                if (throwExceptionOnValidationFailure)
                {
                    throw new Exception(message);
                }
            }
        }

        if (migrations.Where(x => x.IsSnapshot).Any() && migrations.Where(x => x.IsSnapshot).Max(x => x.Version) > migrations.Max(x => x.Version))
        {
            var message = "There are snapshots with version numbers higher than the highest migration version. Snapshots must have version numbers lower than the highest migration version.";
            logger?.LogError(message);
            if (throwExceptionOnValidationFailure)
            {
                throw new Exception(message);
            }
        }
    }

    /// <summary>
    /// Checks that the database is in a valid state for the given migrations. If not, logs errors and throws an exception if throwExceptionOnValidationFailure is true.
    /// 
    /// This method checks the following:
    /// 1. The database must not be ahead of the migrations. If the database is ahead of the migrations, it is likely that the database is migrated for another branch.
    /// 2. Each migration in the database must have a corresponding migration in the migration definitions.
    /// </summary>
    /// <param name="migrations">The defined migrations</param>
    /// <param name="versionInfos">The migration history from the database</param>
    /// <param name="throwExceptionOnValidationFailure"></param>
    /// <exception cref="Exception"></exception>
    private void OnlineMigrationsValidation(IEnumerable<IMigration> migrations, List<VersionInfo> versionInfos, bool throwExceptionOnValidationFailure = true)
    {
        if (versionInfos.Count == 0)
        {
            return;
        }

        if (!migrations.Any() || versionInfos.Max(x => x.Version) > migrations.Max(x => x.Version))
        {
            var message = $"The current database version is {versionInfos.Max(x => x.Version)} but the highest migration version is lower at {(migrations.Any() ? migrations.Max(x => x.Version) : 0)}. This indicates your database is ahead of desired migrated state.";
            _logger.LogError(message);
            if (throwExceptionOnValidationFailure)
            {
                throw new Exception(message);
            }
        }
        foreach (var versionInfo in versionInfos)
        {
            var migration = migrations.FirstOrDefault(x => x.Version == versionInfo.Version && x.IsSnapshot == versionInfo.IsSnapshot);
            if (migration == null)
            {
                var message = $"The current database version is {versionInfo.Version} but there is no migration with that version. This indicates your database is ahead of desired migrated state.";
                _logger.LogError(message);
                if (throwExceptionOnValidationFailure)
                {
                    throw new Exception(message);
                }
            }
            else if (migration.Name != versionInfo.Name)
            {
                var message = $"The migrated database version {versionInfo.Version} has name {versionInfo.Name}, but your current migration definition for version {versionInfo.Version} has name {migration.Name}. This indicates your database is migrated for another branch.";
                _logger.LogError(message);
                if (throwExceptionOnValidationFailure)
                {
                    throw new Exception(message);
                }
            }
        }
    }

}