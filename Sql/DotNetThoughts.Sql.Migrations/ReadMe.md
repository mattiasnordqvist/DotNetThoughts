# SQL Server Migrations

Manage your SQL Server database schema changes using SQL Server Migrations as simple `.sql`-files.

## Runner (example setup)

Create a class that inherits from `SqlMigrationRunner`.
Currently, migrations can only be loaded from Embedded Resources.

```csharp
public class MyMigrationRunner : MigrationRunner<MyMigrationRunner>
{
    public MyMigrationRunner(MigrationRunnerConfiguration<MainMigrationRunner> configuration, ILogger<MyMigrationRunner> logger) : base(configuration, logger)
    {
        // Add migration loaders
        configuration.AddMigrationLoaders(new EmbeddedResourceMigrationLoader(assemblyContainingMyMigrations));
    }
}
```

Use your WebApplicationBuilder to register the migration runner.
The parameter is the name of the configuration section that contains the configuration for the migration runner.

```csharp
    builder.AddMigrationRunner<MyMigrationRunner>("Migrations");
```

Add the configuration section to your `appsettings.json` file.
```json
{
  "Migrations": {
    "AutoCreate": "IF_NOT_EXISTS"
  }
}
```

During startup of your application, run the migrations.
```csharp
    var connectionString = "Server=localhost;Database=MyDatabase;User Id=sa;Password=Password123;";
    var scope = app.Services.CreateScope();
    var runner = scope.ServiceProvider.GetRequiredService<MyMigrationRunner>();
    await runner.RunAsync(connectionString);
```

## Migrations

Migrations are versioned `.sql`-files that are executed in order to update the database schema. Each migration file contains the SQL commands to update the database schema.  

Migration files are named with a version number and a description. The version number is used to determine the order of the migrations.

Migration files are to added as Embedded Resources in the project.

Example: `000_000_000_001_CreateFirstTable.sql`

### Version Number
The migration file name starts with a version number. The version number consists of four parts separated by underscores. You can assign a meaning to each part as you like.

Default rules (not currently configurable):
1. Migrations are run in an order determined by the version number.
2. Version numbers must be unique.
3. Missing versions within a group of 1000s are not allowed.

These rules are enforced to ensure consistent behavior across developer and deployment environments.
Some migration tools allow for migrations to run out of order, but this can lead to unexpected results, especially when multiple developers are working on the same project.

Future considerations:
1. Allow migrations to be run out of order. Just run the ones that have not been run yet and hope for the best.
2. Allow for duplicate version numbers.
3. Allow for missing versions.

### Description
After the version number, there is a description of the migration. Its just a name, for reference purposes only.

### Content
The content of the migration file is the SQL commands to update the database schema. 
You can also insert or update data in the migration file.

The `GO` can be used to separate batches of SQL commands in one file.

### Up/Down
Some migration tools use the concepts of `Up` and `Down` migrations. Since `Up` migrations can be destructive, `Down` migrations are not reliable.  
Here, we only have `Up` migrations. If you need to revert to a previous version, you should rely on a backup of the database, or a new `Up` migration.

## Configuration

### AutoCreate
The `AutoCreate` setting determines whether the database should be created or not when the migrations are run.  
There are 3 options:
- `NEVER`: The database must exist before running the migrations.
- `IF_NOT_EXISTS`: The database is created if it does not exist.
- `DROP_CREATE`: The database is created if it does not exist, and dropped and recreated if it does exist.  
Default: `NEVER`
The `IF_NOT_EXISTS` and `DROP_CREATE` options require the user to have the necessary permissions to create and drop databases, and also have access to the `master`-database, I think.

### VersionInfoTableSchema and VersionInfoTableName
The `VersionInfoTableSchema` and `VersionInfoTableName` settings determine the schema and table name of the table that stores the information about the migrations that have been run.  
Default values are `dbo` and `VersionInfo`, respectively.  
This can't be changed midways through your project without manual interventions in the database.

### DefaultCommandTimeout
The number of seconds to wait for the command to execute, before it times out.  
Default: 200 seconds
Creating indices and adding columns to existing tables etc can take a long time.
Future considerations: Make this value configurable per migration.

### RestoreFromDatabaseOnAutoCreate and SourceDatabaseForRestore
The `RestoreFromDatabaseOnAutoCreate` setting determines whether the database should be restored from another database when the database is created using the `IF_NOT_EXISTS` or `DROP_CREATE` options.  
The `SourceDatabaseForRestore` setting is a connection string to the database to restore from.  
The defaults are `false` and `null`, respectively.

### EnableSnapshot
When the number of migrations grows, the time it takes to run the migrations can become significant.  
When snapshots are enabled, the database schema is restored from the snapshot migration with the highest version, before running the rest of the ordinary migration scripts.

Snapshots represent the state of the database schema at a certain version.   
Snapshot scripts are versioned and named like the usual migration scripts, and placed as Embedded Resources in the project too.  
Snaphot scripts are identified by the suffix `.snapshot.sql`, like `000_000_001_023_MySnapshotName.snapshot.sql`.  
The snapshot version should be the same as the version of the last migration that was run before the snapshot was created.  


