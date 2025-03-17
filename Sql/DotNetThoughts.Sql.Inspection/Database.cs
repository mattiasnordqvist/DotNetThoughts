using DotNetThoughts.Results;
using DotNetThoughts.Sql.Utilities;

using Microsoft.Data.SqlClient;

namespace DotNetThoughts.Sql.Inspection;

public static class Database
{
    public static async Task<Result<Unit>> ExistsAndOnline(string connStr, CancellationToken cancellationToken)
    {
        var db = ConnectionStringUtils.GetInitialCatalog(connStr);
        var masterConnStr = ConnectionStringUtils.ReplaceInitialCatalog(connStr, "master");
        using var conn = new SqlConnection(masterConnStr);
        await conn.OpenAsync(cancellationToken);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT DB_ID('{db}')";
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        if (result == DBNull.Value)
        {
            return Result<Unit>.Error(new DatabaseDoesNotExistError(db));
        }

        using var cmd2 = conn.CreateCommand();
        cmd2.CommandText = $"SELECT DATABASEPROPERTYEX ('{db}', 'Status')";
        var status = await cmd2.ExecuteScalarAsync(cancellationToken);
        if (status?.ToString() != "ONLINE")
        {
            return Result<Unit>.Error(new DatabaseIsNotOnlineError(db));
        }

        return UnitResult.Ok;
    }

    public record DatabaseDoesNotExistError(string DatabaseName) : Error($"The database {DatabaseName} does not exist");
    public record DatabaseIsNotOnlineError(string DatabaseName) : Error($"The database {DatabaseName} is not online");
}