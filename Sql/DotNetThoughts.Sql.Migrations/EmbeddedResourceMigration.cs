using Dapper;

using DotNetThoughts.Results;

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

    public long Version { get; private set; }

    public string Name { get; private set; }

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

    internal static Result<EmbeddedResourceMigration> Create(Assembly assembly, string resource)
    {
        var migration = new EmbeddedResourceMigration(assembly, resource);

        var result = UnitResult.Ok;

        try
        {
           migration.Version = long.Parse(MigrationVersionRegex().Match(resource).Groups[0].Value.Replace("_", ""));
        }
        catch (Exception ex)
        {
            result = result.Or(Result<Unit>.Error(new CouldNotExtractVersionFromFileName(ex, resource)));
        }

        try
        {
            migration.Name = MigrationNameRegex().Match(resource).Groups[0].Value;
        }
        catch (Exception ex)
        {
            result = result.Or(Result<Unit>.Error(new CouldNotExtractNameFromFileName(ex, resource)));
        }

        return result
            .Map(_ => migration);
    }

   

    private EmbeddedResourceMigration(Assembly assembly, string resourceName)
    {
        _assembly = assembly;
        _resourceName = resourceName;
    }

    public record CouldNotExtractVersionFromFileName(Exception Exception, string ResourceName) : ErrorBase();

    public record CouldNotExtractNameFromFileName(Exception Exception, string ResourceName) : ErrorBase();
}