using Dapper;

using Microsoft.Data.SqlClient;

using System.Data.Common;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DotNetThoughts.Sql.Migrations;

/// <summary>
/// A migration that is embedded as a resource in an assembly.
/// </summary>
internal partial class EmbeddedResourceMigration : IMigration
{
    private readonly Assembly _assembly;
    private readonly string _resourceName;

    public EmbeddedResourceMigration(Assembly assembly, string resourceName)
    {
        _assembly = assembly;
        _resourceName = resourceName;
    }

    public long Version => long.Parse(MigrationVersionRegex().Match(_resourceName).Groups[0].Value.Replace("_", ""));

    public string Name => MigrationNameRegex().Match(_resourceName).Groups[0].Value;

    public bool IsSnapshot => _resourceName.Split('.', '_').Contains("snapshot");

    public void Execute(DbConnection c, DbTransaction t, int commandTimeout)
    {
        var sql = GetResourceAsString(_resourceName, _assembly);
        foreach (var batch in Utilities.BatchSplit(sql))
        {
            try
            {
                c.Execute(batch, transaction: t, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                throw new Exception($"""
                    Failed to execute batch.
                    Message: {ex.Message.Trim()}
                    Line (1-indexed): {(ex.LineNumber == 0 ? "Not applicable" : ex.LineNumber.ToString())} 
                    Batch:
                    {string.Join(Environment.NewLine, batch.Split(["\r\n", "\n"], StringSplitOptions.TrimEntries).Select((x, i) =>
                             i + 1 == ex.LineNumber ? $"/* error on this line */ {x}" : $"{x}"
                            ))}
                    """, ex);

            }
        }
    }



    private static string GetResourceAsString(string resource, Assembly assembly)
    {
        var resourceStream = assembly.GetManifestResourceStream(resource) ?? throw new ArgumentException($"Could not find resource: {resource}.");
        return new StreamReader(resourceStream).ReadToEnd();
    }

    [GeneratedRegex("(\\d{3}_){4}")]
    private static partial Regex MigrationVersionRegex();

    [GeneratedRegex("(\\d{3}_){4}.*")]
    private static partial Regex MigrationNameRegex();
}