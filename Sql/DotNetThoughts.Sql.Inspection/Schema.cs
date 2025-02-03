using System.Data.Common;

using Dapper;
namespace DotNetThoughts.Sql.Inspection;
public static class Schema
{
    public static string _tablesSql = """
        SELECT 
            object_id AS ObjectId, 
            name AS Name, 
            schema_id AS SchemaId 
        FROM 
            sys.tables 
        WHERE 
            type = 'U' AND type_desc = 'USER_TABLE';
        
        """;
    public static string _columnsSql = """
        SELECT 
            c.column_id AS ColumnId, 
            c.object_id AS ObjectId, 
            c.name AS ColumnName, 
            c.system_type_id AS SystemTypeId, 
            c.user_type_id AS UserTypeId,
            c.max_length AS MaxLength, 
            c.precision AS Precision, 
            c.scale AS Scale, 
            c.is_nullable AS IsNullable, 
            c.is_identity AS IsIdentity, 
            c.is_computed AS IsComputed 
        FROM 
            sys.columns c 
        INNER JOIN 
            sys.tables t ON t.object_id = c.object_id 
        WHERE 
            t.type = 'U';
        """;
    public static string _schemasSql = """
        SELECT 
            schema_id AS SchemaId, 
            name AS SchemaName 
        FROM 
            sys.schemas;
        """;
    public static string _typesSql = """
        SELECT
            name AS Name, 
            system_type_id AS SystemTypeId,
            user_type_id AS UserTypeId
        FROM
            sys.types;
        """;
    public static string _indicesSql = """
        SELECT 
            object_id AS ObjectId, 
            index_id AS IndexId,
            name as Name, 
            type_desc as TypeDesc,
            is_unique AS IsUnique, 
            is_primary_key as IsPrimaryKey, 
            is_unique_constraint as IsUniqueConstraint,
            filter_definition as FilterDefinition
        FROM 
            sys.indexes;
        """;
    public static string _indexColumnsSql = """
        SELECT 
            object_id AS ObjectId, 
            index_id AS IndexId,
            column_id AS ColumnId,
            key_ordinal AS KeyOrdinal,
            is_descending_key AS IsDescendingKey,
            is_included_column AS IsIncludedColumn
        FROM
            sys.index_columns;
        """;
    public static string _foreignKeysSql = """
        SELECT
            name AS ForeignKeyName,
            object_id AS ObjectId,
            parent_object_id AS ParentObjectId,
            referenced_object_id AS ReferencedObjectId,
            delete_referential_action AS DeleteReferentialAction,
            update_referential_action AS UpdateReferentialAction
        FROM
            sys.foreign_keys;
        """;
    public static string _foreignKeysColumnsSql = """
        SELECT
            constraint_object_id AS ConstraintObjectId,
            constraint_column_id AS ConstraintColumnId,
            parent_object_id AS ParentObjectId,
            parent_column_id AS ParentColumnId,	
            referenced_object_id AS ReferencedObjectId,
            referenced_column_id AS ReferencedColumnId
        FROM
            sys.foreign_key_columns;
        """;

    public static string _defaultConstraintsSql = """
        SELECT
               dc.parent_object_id AS ParentObjectId,
               dc.parent_column_id AS ParentColumnId,
               dc.name AS ConstraintName,
               dc.definition AS ConstraintDefinition
        FROM sys.default_constraints dc;
        """;

    public static string _identityColumnsSql = """
        SELECT 
            object_id as ObjectId, 
            column_id as ColumnId, 
            seed_value as SeedValue, 
            increment_value as IncrementValue
        FROM sys.identity_columns;
        """;

    public static string _viewsSql = """
        SELECT definition AS Definition, schema_id AS SchemaId
        FROM sys.sql_modules m
        INNER JOIN sys.views v on v.object_id = m.object_id
        WHERE v.is_ms_shipped = 0;
        """;

    public static string _triggersSql = """
        SELECT definition AS Definition
        FROM sys.sql_modules m
        inner join sys.triggers v on v.object_id = m.object_id;
        """;

    public static string _computedColumnsSql = """
        select definition AS Definition, is_persisted AS IsPersisted, object_id AS ObjectId, column_id AS ColumnId
        from sys.computed_columns;
        """;

    public static string _checkConstraintsSql = """
        select 
        	cc.definition AS Definition, 
        	cc.name AS Name, 
        	cc.object_id AS ObjectId, 
        	cc.schema_id AS SchemaId, 
        	cc.parent_object_id as ParentObjectId, 
        	cc.parent_column_id as ParentColumnId
        from sys.check_constraints cc;
        """;

    public record TableInfo(int ObjectId, string Name, int SchemaId);
    public record SchemaInfo(int SchemaId, string SchemaName);
    public record ColumnInfo(
        int ColumnId,
        int ObjectId,
        string ColumnName,
        byte SystemTypeId,
        int UserTypeId,
        short MaxLength,
        byte Precision,
        byte Scale,
        bool IsNullable,
        bool IsIdentity,
        bool IsComputed);
    public record TypeInfo(string Name, byte SystemTypeId, int UserTypeId);
    public record IndexInfo(int ObjectId, int IndexId, string Name, string TypeDesc, bool IsUnique, bool IsPrimaryKey, bool IsUniqueConstraint, string? FilterDefinition);
    public record IndexColumnsInfo(int ObjectId, int IndexId, int ColumnId, byte KeyOrdinal, bool IsDescendingKey, bool IsIncludedColumn);
    public record ForeignKeyInfo(string ForeignKeyName, int ObjectId, int ParentObjectId, int ReferencedObjectId, byte DeleteReferentialAction, byte UpdateReferentialAction);
    public record ForeignKeyColumnsInfo(int ConstraintObjectId, int ConstraintColumnId, int ParentObjectId, int ParentColumnId, int ReferencedObjectId, int ReferencedColumnId);
    public record DefaultConstraintInfo(int ParentObjectId, int ParentColumnId, string ConstraintName, string ConstraintDefinition);
    public record IdentityColumnInfo(int ObjectId, int ColumnId, object SeedValue, object IncrementValue);
    public record ViewInfo(string Definition, int SchemaId);
    public record TriggerInfo(string Definition);
    public record ComputedColumnInfo(string Definition, bool IsPersisted, int ObjectId, int ColumnId);
    public record CheckConstraintInfo(string Definition, string Name, int ObjectId, int SchemaId, int ParentObjectId, int ParentColumnId);
    public static async Task<FlatDatabaseSchema> GetSchemaAsync(this DbConnection connection)
    {
        var sql = _schemasSql +
            _tablesSql +
            _columnsSql +
            _typesSql +
            _indicesSql +
            _indexColumnsSql +
            _foreignKeysSql +
            _foreignKeysColumnsSql +
            _defaultConstraintsSql +
            _identityColumnsSql +
            _viewsSql +
            _triggersSql +
            _computedColumnsSql +
            _checkConstraintsSql;
        using var results = await connection.QueryMultipleAsync(sql);
        var schemas = await results.ReadAsync<SchemaInfo>();
        var tables = await results.ReadAsync<TableInfo>();
        var columns = await results.ReadAsync<ColumnInfo>();
        var types = await results.ReadAsync<TypeInfo>();
        var indices = await results.ReadAsync<IndexInfo>();
        var indexColumns = await results.ReadAsync<IndexColumnsInfo>();
        var foreignKeys = await results.ReadAsync<ForeignKeyInfo>();
        var foreignKeyColumns = await results.ReadAsync<ForeignKeyColumnsInfo>();
        var defaultConstraints = await results.ReadAsync<DefaultConstraintInfo>();
        var identityColumns = await results.ReadAsync<IdentityColumnInfo>();
        var views = await results.ReadAsync<ViewInfo>();
        var triggers = await results.ReadAsync<TriggerInfo>();
        var computedColumns = await results.ReadAsync<ComputedColumnInfo>();
        var checkConstraints = await results.ReadAsync<CheckConstraintInfo>();

        return new FlatDatabaseSchema(
            schemas.ToArray(),
            tables.ToArray(),
            columns.ToArray(),
            types.ToArray(),
            indices.ToArray(),
            indexColumns.ToArray(),
            foreignKeys.ToArray(),
            foreignKeyColumns.ToArray(),
            defaultConstraints.ToArray(),
            identityColumns.ToArray(),
            views.ToArray(),
            triggers.ToArray(),
            computedColumns.ToArray(),
            checkConstraints.ToArray()
            );
    }

    public record FlatDatabaseSchema(
        SchemaInfo[] SchemaInfos,
        TableInfo[] TableInfos,
        ColumnInfo[] ColumnInfos,
        TypeInfo[] TypeInfos,
        IndexInfo[] IndexInfos,
        IndexColumnsInfo[] IndexColumnsInfos,
        ForeignKeyInfo[] ForeignKeyInfos,
        ForeignKeyColumnsInfo[] ForeignKeyColumnsInfos,
        DefaultConstraintInfo[] DefaultConstraintInfos,
        IdentityColumnInfo[] IdentityColumnInfos,
        ViewInfo[] ViewInfos,
        TriggerInfo[] TriggerInfos,
        ComputedColumnInfo[] ComputedColumnInfos,
        CheckConstraintInfo[] CheckConstraintInfos);
}
