using Microsoft.Data.SqlClient;

namespace DotNetThoughts.Sql.Utilities;
public static class ConnectionStringUtils
{

    public static bool IsLocalhost(string connectionString)
    {
        var dataSource = new SqlConnectionStringBuilder(connectionString).DataSource;
        return IsLocalhostAddress(dataSource);
    }

    private static bool IsLocalhostAddress(string hostNameOrAddress)
    {
        return hostNameOrAddress.StartsWith("localhost")
             || hostNameOrAddress.StartsWith("(local)")
             || hostNameOrAddress.StartsWith("127.0.0.1")
             || hostNameOrAddress == "."
             || hostNameOrAddress == Environment.MachineName
             || hostNameOrAddress.Contains('\\') && hostNameOrAddress.Substring(0, hostNameOrAddress.IndexOf("\\")).Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase);


    }
    /// <summary>
    /// Parses a connectionstring and returns the Initial Catalog part
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static string GetInitialCatalog(string connectionString)
        => new SqlConnectionStringBuilder(connectionString).InitialCatalog;

    /// <summary>
    /// Replaces the Initial Catalog part of a connectionstring
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="newInitialCatalog"></param>
    /// <returns></returns>
    public static string ReplaceInitialCatalog(string connectionString, string newInitialCatalog)
        => new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = newInitialCatalog
        }.ConnectionString;
}
