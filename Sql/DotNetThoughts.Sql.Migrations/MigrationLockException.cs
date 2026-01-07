namespace DotNetThoughts.Sql.Migrations;

/// <summary>
/// Exception thrown when a migration lock cannot be acquired within the configured timeout.
/// </summary>
public class MigrationLockException : Exception
{
    public string DatabaseName { get; }
    public string LockResource { get; }
    public int TimeoutMs { get; }
    public int ResultCode { get; }

    public MigrationLockException(string databaseName, string lockResource, int timeoutMs, int resultCode)
        : base(CreateMessage(databaseName, lockResource, timeoutMs, resultCode))
    {
        DatabaseName = databaseName;
        LockResource = lockResource;
        TimeoutMs = timeoutMs;
        ResultCode = resultCode;
    }

    private static string CreateMessage(string databaseName, string lockResource, int timeoutMs, int resultCode)
    {
        var resultDescription = resultCode switch
        {
            -1 => "The lock request timed out.",
            -2 => "The lock request was canceled.",
            -3 => "The lock request was chosen as a deadlock victim.",
            -999 => "Parameter validation or other call error.",
            _ => $"Unknown error (result code: {resultCode})."
        };

        return $"""
            Failed to acquire migration lock for database '{databaseName}'.
            Lock resource: {lockResource}
            Timeout: {timeoutMs}ms
            Result: {resultDescription}
            
            This typically occurs when another migration process is running against the same database.
            If you expect migrations to take longer than {timeoutMs}ms, consider increasing the 
            MigrationLockTimeoutMs option in MigrationRunnerOptions.
            """;
    }
}
