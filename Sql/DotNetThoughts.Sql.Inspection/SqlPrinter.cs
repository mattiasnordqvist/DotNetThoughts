using System.Data;
using System.Text;

using static DotNetThoughts.Sql.Inspection.Schema;

namespace DotNetThoughts.Sql.Inspection;

public class SqlPrinter
{
    public class PrintOptions
    {
        public HashSet<string> IgnoreTables = [];
        public Dictionary<string, bool> IgnoreSchemas = new Dictionary<string, bool>
        {
            { "dbo", false },
            { "sys", true },
            { "guest", true },
            { "INFORMATION_SCHEMA", true },
            { "db_owner", true },
            { "db_accessadmin", true },
            { "db_securityadmin", true },
            { "db_ddladmin", true },
            { "db_backupoperator", true },
            { "db_datareader", true },
            { "db_datawriter", true },
            { "db_denydatareader", true },
            { "db_denydatawriter", true }
        };
        public bool PrintObjectsReferencingIgnoredObjects = true;
    }

    public static string PrintAsExecutable(FlatDatabaseSchema db, Action<PrintOptions> configure)
    {
        var options = new PrintOptions();
        configure(options);
        var sb = new StringBuilder();

        var ignoreSchemaIds = db.SchemaInfos
            .Where(x => options.IgnoreSchemas.ContainsKey(x.SchemaName))
            .Select(x => x.SchemaId)
            .ToHashSet();

        var ignoreSchemaContentIds = db.SchemaInfos
            .Where(x => options.IgnoreSchemas.TryGetValue(x.SchemaName, out var ignoreSchemaContent) && ignoreSchemaContent)
            .Select(x => x.SchemaId)
            .ToHashSet();

        var ignoreTableIds = options.IgnoreTables.Select(x =>
            db.TableInfos.Single(y => y.Name == x).ObjectId).ToList()
            .Union(db.TableInfos.Where(x => ignoreSchemaContentIds.Contains(x.SchemaId)).Select(x => x.ObjectId).ToHashSet());

        foreach (var schema in db.SchemaInfos
            .Where(x => !ignoreSchemaIds.Contains(x.SchemaId))
            .OrderBy(x => x.SchemaName))
        {
            PrintSchema(sb, schema);
            sb.AppendLine("GO");
        }
        foreach (var table in db.TableInfos
            .Where(x => !options.IgnoreTables.Contains(x.Name))
            .OrderBy(x => x.Name))
        {
            PrintTable(sb, db, table);
            sb.AppendLine("GO");
        }

        foreach (var table in db.TemporalTableInfos
            .Where(x => !options.IgnoreTables.Contains(x.Name) && x.TemporalType == 2)
            .OrderBy(x => x.Name))
        {
            PrintTemporalTable(sb, db, table);
            sb.AppendLine("GO");
        }

        foreach (var defaultConstraint in db.DefaultConstraintInfos
            .Where(x => options.PrintObjectsReferencingIgnoredObjects || !ignoreTableIds.Contains(x.ParentObjectId))
            .OrderBy(x => x.ConstraintName))
        {
            PrintDefaultConstraint(sb, db, defaultConstraint);
            sb.AppendLine("GO");
        }

        foreach (var foreignkey in db.ForeignKeyInfos
            .Where(x => options.PrintObjectsReferencingIgnoredObjects || !ignoreTableIds.Contains(x.ParentObjectId))
            .OrderBy(x => x.ForeignKeyName))
        {
            PrintForeignKey(sb, db, foreignkey);
            sb.AppendLine("GO");
        }
        foreach (var uc in db.IndexInfos
            .Where(x => options.PrintObjectsReferencingIgnoredObjects || !ignoreTableIds.Contains(x.ObjectId))
            .Where(x => x.IsUniqueConstraint).OrderBy(x => x.Name))
        {
            PrintUniqueConstraints(sb, db, uc);
            sb.AppendLine("GO");
        }
        foreach (var ix in db.IndexInfos
            .Where(x => options.PrintObjectsReferencingIgnoredObjects || !ignoreTableIds.Contains(x.ObjectId))
            .Where(x => !x.IsPrimaryKey && !x.IsUniqueConstraint
            && x.TypeDesc != "HEAP")
            .OrderBy(x => x.Name))
        {
            if (PrintIndex(sb, db, ix))
            {
                sb.AppendLine("GO");
            }
        }

        var viewObjectIds = db.ViewInfos
            .Select(x => x.object_id)
            .ToHashSet();

        var view_viewDependencies = db.DependencyInfos
            .Where(x => viewObjectIds.Contains(x.referenced_id) && viewObjectIds.Contains(x.referencing_id));

        foreach (var view in db.ViewInfos
            .Where(x => options.PrintObjectsReferencingIgnoredObjects || !ignoreSchemaIds.Contains(x.SchemaId))
            .OrderBy(x => x, new ViewDependencySorter(view_viewDependencies))
            .ThenBy(x => x.name))
        {
            sb.AppendLine(view.Definition);
            sb.AppendLine("GO");
        }

        foreach (var trigger in db.TriggerInfos.OrderBy(x => x.Definition))
        {
            sb.AppendLine(trigger.Definition);
            sb.AppendLine("GO");
        }

        foreach (var checkConstraint in db.CheckConstraintInfos
            .Where(x => options.PrintObjectsReferencingIgnoredObjects || !ignoreTableIds.Contains(x.ParentObjectId))
            .OrderBy(x => x.Name))
        {
            var table = db.TableInfos.Single(x => x.ObjectId == checkConstraint.ParentObjectId);
            var schema = db.SchemaInfos.Single(x => x.SchemaId == table.SchemaId);
            sb.AppendLine($"ALTER TABLE [{schema.SchemaName}].[{table.Name}] WITH CHECK ADD CONSTRAINT [{checkConstraint.Name}] CHECK {checkConstraint.Definition};");
            sb.AppendLine("GO");

            sb.AppendLine($"ALTER TABLE [{schema.SchemaName}].[{table.Name}] CHECK CONSTRAINT [{checkConstraint.Name}]");
            sb.AppendLine("GO");
        }

        return sb.ToString();
    }

    public static void PrintSchema(StringBuilder sb, SchemaInfo schema)
    {
        sb.AppendLine($"CREATE SCHEMA [{schema.SchemaName}];");
        sb.AppendLine();
    }

    public static void PrintTable(StringBuilder sb, FlatDatabaseSchema db, TableInfo table)
    {
        var schema = db.SchemaInfos.Single(x => x.SchemaId == table.SchemaId);
        var columns = db.ColumnInfos.Where(x => x.ObjectId == table.ObjectId).OrderBy(x => x.ColumnId).ToList();
        sb.AppendLine($"CREATE TABLE [{schema.SchemaName}].[{table.Name}] (");
        sb.AppendLine(string.Join(",\n", columns.Select(x => "    " + PrintColumn(db, x))));
        sb.AppendLine(");");
        sb.AppendLine();

        if (db.IndexInfos.Any(x => x.ObjectId == table.ObjectId && x.IsPrimaryKey))
        {
            var pk = db.IndexInfos.Single(x => x.ObjectId == table.ObjectId && x.IsPrimaryKey);
            var pkColumns = db.IndexColumnsInfos
                .Where(x => x.IndexId == pk.IndexId && table.ObjectId == x.ObjectId)
                .OrderBy(x => x.KeyOrdinal)
                .Select(x => new { Column = db.ColumnInfos.Single(y => y.ObjectId == table.ObjectId && y.ColumnId == x.ColumnId), x.IsDescendingKey })
                .ToList();
            sb.AppendLine($"ALTER TABLE [{schema.SchemaName}].[{table.Name}] ADD CONSTRAINT [{pk.Name}] PRIMARY KEY {pk.TypeDesc} ({string.Join(", ", pkColumns.Select(x => $"[{x.Column.ColumnName}] {(x.IsDescendingKey ? "DESC" : "ASC")}"))});");
        }
        if (db.IndexInfos.Any(x => x.ObjectId == table.ObjectId && x.TypeDesc == "HEAP"))
        {
            sb.AppendLine($"-- Table has no PRIMARY KEY. It is a HEAP.");
        }

        sb.AppendLine();
    }

    public static void PrintTemporalTable(StringBuilder sb, FlatDatabaseSchema db, TemporalTableInfo table)
    {
        if (table.HistoryTableId is null)
            return;

        var schema = db.SchemaInfos.Single(x => x.SchemaId == table.SchemaId);
        var historyTable = db.TableInfos.Single(x => x.ObjectId == table.HistoryTableId.Value);
        var period = db.PeriodInfos.SingleOrDefault(x => x.ObjectId == table.ObjectId && x.PeriodType == 1);

        if (period is not null)
        {
            var historySchema = db.SchemaInfos.Single(x => x.SchemaId == historyTable.SchemaId);
            var startColumn = db.ColumnInfos.Single(x => x.ObjectId == table.ObjectId && x.ColumnId == period.StartColumnId);
            var endColumn = db.ColumnInfos.Single(x => x.ObjectId == table.ObjectId && x.ColumnId == period.EndColumnId);

            sb.AppendLine($"ALTER TABLE [{schema.SchemaName}].[{table.Name}] ADD PERIOD FOR SYSTEM_TIME ([{startColumn.ColumnName}], [{endColumn.ColumnName}]);");
            sb.AppendLine($"ALTER TABLE [{schema.SchemaName}].[{table.Name}] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [{historySchema.SchemaName}].[{historyTable.Name}]));");
            sb.AppendLine();
        }
    }

    public static void PrintDefaultConstraint(StringBuilder sb, FlatDatabaseSchema db, DefaultConstraintInfo dc)
    {
        var table = db.TableInfos.Single(x => x.ObjectId == dc.ParentObjectId);
        var schema = db.SchemaInfos.Single(x => x.SchemaId == table.SchemaId);
        var column = db.ColumnInfos.Single(x => x.ObjectId == table.ObjectId && x.ColumnId == dc.ParentColumnId);
        sb.AppendLine($"ALTER TABLE [{schema.SchemaName}].[{table.Name}] ADD CONSTRAINT [{dc.ConstraintName}] DEFAULT {dc.ConstraintDefinition} FOR [{column.ColumnName}];");
        sb.AppendLine();
    }

    public static void PrintForeignKey(StringBuilder sb, FlatDatabaseSchema db, ForeignKeyInfo fk)
    {
        var table = db.TableInfos.Single(x => x.ObjectId == fk.ParentObjectId);
        var schema = db.SchemaInfos.Single(x => x.SchemaId == table.SchemaId);
        var fkColumns = db.ForeignKeyColumnsInfos
               .Where(x => x.ConstraintObjectId == fk.ObjectId)
               .OrderBy(x => x.ConstraintColumnId)
               .Select(x => new
               {
                   FKColumns = db.ColumnInfos.Single(y => y.ObjectId == table.ObjectId && y.ColumnId == x.ParentColumnId),
                   ReferencesObject = db.TableInfos.Single(y => y.ObjectId == x.ReferencedObjectId),
                   ReferencedColumns = db.ColumnInfos.Single(y => y.ObjectId == x.ReferencedObjectId && y.ColumnId == x.ReferencedColumnId),
               })
               .ToList();
        sb.AppendLine($"ALTER TABLE [{schema.SchemaName}].[{table.Name}] WITH CHECK ADD CONSTRAINT [{fk.ForeignKeyName}] FOREIGN KEY ({string.Join(", ", fkColumns.Select(x => $"[{x.FKColumns.ColumnName}]"))})");
        sb.AppendLine($"REFERENCES [{db.SchemaInfos.Single(x => x.SchemaId == fkColumns.First().ReferencesObject.SchemaId).SchemaName}].[{fkColumns.First().ReferencesObject.Name}] ({string.Join(", ", fkColumns.Select(x => $"[{x.ReferencedColumns.ColumnName}]"))})");
        if (fk.DeleteReferentialAction != 0)
        {
            sb.AppendLine($"ON DELETE {fk.DeleteReferentialAction switch { 1 => "CASCADE", 2 => "SET NULL", 3 => "SET DEFAULT", _ => throw new Exception() }}");
        }
        if (fk.UpdateReferentialAction != 0)
        {
            sb.AppendLine($"ON UPDATE {fk.UpdateReferentialAction switch { 1 => "CASCADE", 2 => "SET NULL", 3 => "SET DEFAULT", _ => throw new Exception() }}");
        }
        sb.AppendLine();
    }

    public static void PrintUniqueConstraints(StringBuilder sb, FlatDatabaseSchema db, IndexInfo uc)
    {
        var table = db.TableInfos.Single(x => x.ObjectId == uc.ObjectId);
        var schema = db.SchemaInfos.Single(x => x.SchemaId == table.SchemaId);
        var ucColumns = db.IndexColumnsInfos
             .Where(x => x.IndexId == uc.IndexId && table.ObjectId == x.ObjectId)
             .OrderBy(x => x.KeyOrdinal)
             .Select(x => new { Column = db.ColumnInfos.Single(y => y.ObjectId == table.ObjectId && y.ColumnId == x.ColumnId), x.IsDescendingKey })
             .ToList();
        sb.AppendLine($"ALTER TABLE [{schema.SchemaName}].[{table.Name}] ADD CONSTRAINT [{uc.Name}] UNIQUE {uc.TypeDesc} ({string.Join(", ", ucColumns.Select(x => $"[{x.Column.ColumnName}] {(x.IsDescendingKey ? "DESC" : "ASC")}"))});");
        sb.AppendLine();
    }

    public static bool PrintIndex(StringBuilder sb, FlatDatabaseSchema db, IndexInfo ix)
    {
        var table = db.TableInfos.SingleOrDefault(x => x.ObjectId == ix.ObjectId);
        if (table == null)
        {
            return false;
        }
        var schema = db.SchemaInfos.Single(x => x.SchemaId == table.SchemaId);
        var ixColumns = db.IndexColumnsInfos
            .Where(x => x.IndexId == ix.IndexId && table.ObjectId == x.ObjectId && !x.IsIncludedColumn)
            .OrderBy(x => x.KeyOrdinal)
            .Select(x => new { Column = db.ColumnInfos.Single(y => y.ObjectId == table.ObjectId && y.ColumnId == x.ColumnId), x.IsDescendingKey })
            .ToList();
        var includedColumns = db.IndexColumnsInfos
            .Where(x => x.IndexId == ix.IndexId && table.ObjectId == x.ObjectId && x.IsIncludedColumn)
            .OrderBy(x => x.KeyOrdinal)
            .Select(x => db.ColumnInfos.Single(y => y.ObjectId == table.ObjectId && y.ColumnId == x.ColumnId))
            .ToList();
        sb.Append($"CREATE{(ix.IsUnique ? " UNIQUE" : "")} {ix.TypeDesc} INDEX [{ix.Name}] ON [{schema.SchemaName}].[{table.Name}] ({string.Join(", ", ixColumns.Select(x => $"[{x.Column.ColumnName}] {(x.IsDescendingKey ? "DESC" : "ASC")}"))})");

        var includeStatement = includedColumns.Count != 0 ? $" INCLUDE ({string.Join(", ", includedColumns.Select(x => $"[{x.ColumnName}]"))})" : "";
        var filterStatement = ix.FilterDefinition != null ? $" WHERE {ix.FilterDefinition}" : "";
        sb.Append(includeStatement);
        sb.Append(filterStatement);
        sb.AppendLine(";");
        sb.AppendLine();
        return true;
    }

    public static string PrintColumn(FlatDatabaseSchema db, ColumnInfo column)
    {
        var type = db.TypeInfos.Single(x => x.SystemTypeId == column.SystemTypeId && x.UserTypeId == column.UserTypeId);
        var identityInfo = db.IdentityColumnInfos.SingleOrDefault(x => x.ObjectId == column.ObjectId && x.ColumnId == column.ColumnId);
        var computedColumnInfo = db.ComputedColumnInfos.SingleOrDefault(x => x.ObjectId == column.ObjectId && x.ColumnId == column.ColumnId);

        var typeName = type.Name.ToUpper();
        var maxLength = column.MaxLength == -1
            ? "MAX"
            : type.Name == "nvarchar"
                ? $"{column.MaxLength / 2}"
                : $"{column.MaxLength}";
        var length = type.Name switch
        {
            "nvarchar" => $"({maxLength})",
            "decimal" => $"({column.Precision},{column.Scale})",
            "varchar" => $"({maxLength})",
            _ => ""
        };


        if (computedColumnInfo != null)
        {
            var @null = column.IsNullable ? "" : "NOT NULL";

            return $"[{column.ColumnName}] AS {computedColumnInfo.Definition} {(computedColumnInfo.IsPersisted ? " PERSISTED" : "")} {@null}";
        }
        else
        {
            var @null = column.IsNullable ? "NULL" : "NOT NULL";

            return $"[{column.ColumnName}] [{typeName}]{length}{(identityInfo != null ? $" IDENTITY({identityInfo.SeedValue},{identityInfo.IncrementValue})" : "")} {@null}";
        }
    }
}